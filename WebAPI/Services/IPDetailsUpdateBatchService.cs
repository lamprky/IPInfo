using System;
using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Models.Database;
using WebAPI.Models.Service;
using WebAPI.Repositories.Interfaces;
using WebAPI.Services.Interfaces;
using WebAPI.Common;
using WebAPI.Models;
using System.Data.SqlClient;
using WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using IPInfo.Interfaces;

namespace WebAPI.Services
{
    public class IPDetailsUpdateBatchService : IIPDetailsUpdateBatchService
    {
        private readonly int cacheExpirationMins = Startup.CacheExpirationMins;
        private readonly int batchRecords = Startup.BatchRecords;
        private readonly string strConnString = Startup.ConnectionString;
            
        private IMemoryCache _cache;
        private readonly IUnitOfWork _uow;
        private readonly IBatchDetailsRepository _batchDetailsRepository;
        private readonly IIPDetailsRepository _ipDetailsRepository;

        public IPDetailsUpdateBatchService(IMemoryCache memoryCache,
            IUnitOfWork uow,
            IBatchDetailsRepository batchDetailsRepository,
            IIPDetailsRepository ipDetailsRepository)
        {
            _cache = memoryCache;
            _uow = uow;
            _batchDetailsRepository = batchDetailsRepository;
            _ipDetailsRepository = ipDetailsRepository;
        }

        public async Task<BatchDetailModel> GetJobProgress(Guid batchId)
        {
            using (_uow)
            {
                var batchDetails = await _batchDetailsRepository.GetById(batchId);
                if (batchDetails != null)
                    return batchDetails.ToBatchDetailModel();
                else
                    return null;
            }
        }

        public async Task<Guid> UpdateIPDetails(List<IPDetailsModel> details)
        {
            Guid batchId = new Guid();
            try
            {
                details.ValidateDetails();

                using (_uow)
                {
                    batchId = await CreateBatchDetail(details.Count);
                }
                Task execution = RunBatch(batchId, details);
            }
            catch
            {
                throw;
            }

            return batchId;
        }

        private async Task<Guid> CreateBatchDetail(int noOfDetails)
        {
            BatchDetailsDTO dbObj = new BatchDetailsDTO
            {
                ID = Guid.NewGuid(),
                No_of_Updates = noOfDetails,
                No_of_Updates_Processed = 0,
                StartTime = DateTime.Now,
                EndTime = null
            };
            await _batchDetailsRepository.Insert(dbObj);
            await _uow.Save();

            return dbObj.ID;
        }

        private async Task RunBatch(Guid batchId, List<IPDetailsModel> details)
        {
            int maxLoops = (int)(details.Count / batchRecords) + (details.Count % batchRecords > 0 ? 1 : 0);
            int loopsCounter = 0;
            using (SqlConnection connection = new SqlConnection(strConnString))
            {
                while (loopsCounter < maxLoops)
                {
                    await connection.OpenAsync();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        await ProcessRecords(batchId, details, maxLoops, loopsCounter, connection, transaction);

                        transaction.Commit();

                        loopsCounter++;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private async Task ProcessRecords(Guid batchId, List<IPDetailsModel> details, int maxLoops, int loopsCounter,
            IDbConnection connection, IDbTransaction transaction)
        {
            int recordsToTake = (loopsCounter + 1 == maxLoops) ? details.Count % batchRecords : batchRecords;
            var detailsInProcess = details.Skip(loopsCounter * batchRecords).Take(recordsToTake).ToList();

            //find them already exists in db
            var detailsInDb = GetExistingIPDetailRecords(detailsInProcess, connection, transaction);

            //update them
            Task updateRecs = UpdateExistingIPDetailRecords(detailsInProcess, detailsInDb, connection, transaction);

            //for the rest do insert
            Task insertRecs = InsertNotExistingIPDetailRecords(detailsInProcess, detailsInDb, connection, transaction);

            //update batch record
            var processedRecords = (loopsCounter * batchRecords) + recordsToTake;
            DateTime? endDate = (loopsCounter + 1 == maxLoops) ? DateTime.Now : (DateTime?)null;
            Task updateBatchRec = UpdateBatchRecord(batchId, processedRecords, endDate, connection, transaction);

            await Task.WhenAll(new List<Task> { updateRecs, insertRecs, updateBatchRec });
        }

        private List<string> GetExistingIPDetailRecords(List<IPDetailsModel> detailsInProcess, 
            IDbConnection connection, IDbTransaction transaction)
        {
            //var detailsInDb = await _ipDetailsRepository.Get(x => detailsInProcess.Select(y => y.IP).Contains(x.IP));

            List<string> detailsInDb = _ipDetailsRepository.GetByIps(detailsInProcess.Select(x => x.IP).ToList(), connection, transaction).Select(x => x.IP).ToList();
            return detailsInDb;
        }

        private async Task UpdateExistingIPDetailRecords(List<IPDetailsModel> detailsInProcess, List<string> detailsInDb, 
            IDbConnection connection, IDbTransaction transaction)
        {
            var detailsToUpdate = detailsInProcess.Where(x => detailsInDb.Contains(x.IP)).ToList();
            //Parallel.ForEach(detailsToUpdate, x => _ipDetailsRepository.Update(x.ToDetailsDTO()));

            await Task.Delay(5000);

            Parallel.ForEach(detailsToUpdate, x =>
            {
                _ipDetailsRepository.UpdateDetail(x.ToDetailsDTO(), connection, transaction);
                UpdateCache(x.IP, x);
            });
        }

        private async Task InsertNotExistingIPDetailRecords(List<IPDetailsModel> detailsInProcess, List<string> detailsInDb, 
            IDbConnection connection, IDbTransaction transaction)
        {
            var detailsToInsert = detailsInProcess.Where(x => !detailsInDb.Contains(x.IP)).ToList();
            //Parallel.ForEach(detailsToInsert, x => _ipDetailsRepository.Insert(x.ToDetailsDTO()));

            await Task.Delay(10000);

            Parallel.ForEach(detailsToInsert, x =>
            {
                _ipDetailsRepository.InsertDetail(x.ToDetailsDTO(), connection, transaction);
                UpdateCache(x.IP, x);
            });
        }

        private async Task UpdateBatchRecord(Guid batchId, int processedRecords, DateTime? endDate, 
            IDbConnection connection, IDbTransaction transaction)
        {
            //var batchDetail = await _batchDetailsRepository.GetById(batchId);
            //batchDetail.No_of_Updates_Processed = processedRecords;
            //batchDetail.EndTime = endDate;
            //_batchDetailsRepository.Update(batchDetail);

            await Task.Delay(15000);

            await _batchDetailsRepository.UpdateDetail(processedRecords, endDate, batchId, connection, transaction);
        }

        private void UpdateCache(string ip, IPDetails details)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpirationMins));
            _cache.Set(ip, details, cacheEntryOptions);
        }
    }
}
