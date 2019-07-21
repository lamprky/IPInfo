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

namespace WebAPI.Services
{
    public class IPDetailsUpdateBatchService : IIPDetailsUpdateBatchService
    {
        private int cacheExpirationMins = int.Parse(ConfigurationManager.AppSettings.Get("cacheExpirationMins"));
        private int batchRecords = int.Parse(ConfigurationManager.AppSettings.Get("batchRecords"));
        private string strConnString = "data source=SANDY\\SQLEXPRESS; Initial Catalog=IPInfo;Integrated Security=True;MultipleActiveResultSets=True";

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

                    int recordsToTake = (loopsCounter + 1 == maxLoops) ? details.Count % batchRecords : batchRecords;
                    var detailsInProcess = details.Skip(loopsCounter * batchRecords).Take(recordsToTake).ToList();

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        //find them already exists in db
                        var detailsInDb = await GetExistingIPDetailRecords(detailsInProcess, connection, transaction);

                        //update them
                        Task updateRecs = UpdateExistingIPDetailRecords(detailsInProcess, detailsInDb, connection, transaction);

                        //for the rest do insert
                        Task insertRecs = InsertNotExistingIPDetailRecords(detailsInProcess, detailsInDb, connection, transaction);

                        //update batch record
                        var processedRecords = (loopsCounter * batchRecords) + recordsToTake;
                        DateTime? endDate = (loopsCounter + 1 == maxLoops) ? DateTime.Now : (DateTime?)null;
                        Task updateBatchRec = UpdateBatchRecord(batchId, processedRecords, endDate, connection, transaction);

                        await Task.WhenAll(new List<Task> { updateRecs, insertRecs, updateBatchRec });

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

        private async Task<List<string>> GetExistingIPDetailRecords(List<IPDetailsModel> detailsInProcess, IDbConnection connection, IDbTransaction transaction)
        {
            //var detailsInDb = await _ipDetailsRepository.Get(x => detailsInProcess.Select(y => y.IP).Contains(x.IP));

            //List<string> detailsInDb = new List<string>();
            //var parameters = new string[detailsInProcess.Count];
            //var cmd = new SqlCommand();
            //for (int i = 0; i < detailsInProcess.Count; i++)
            //{
            //    parameters[i] = string.Format("@IP{0}", i);
            //    cmd.Parameters.AddWithValue(parameters[i], detailsInProcess[i].IP);
            //}
            //cmd.Connection = connection;
            //cmd.Transaction = transaction;
            //cmd.CommandText = string.Format("SELECT IP FROM IPDetails WHERE IP IN ({0})", string.Join(", ", parameters));

            //using (SqlDataReader reader = cmd.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        detailsInDb.Add(reader.GetString(0));
            //    }
            //}

            string sql = "SELECT * FROM IPDetails WHERE IP IN @ips";
            List<string> detailsInDb = connection.Query<IPDetailsDTO>(sql, new { ips = detailsInProcess.Select(x => x.IP).ToArray() }, transaction).ToList().Select(x => x.IP).ToList();
            return detailsInDb;
        }

        private async Task UpdateExistingIPDetailRecords(List<IPDetailsModel> detailsInProcess, List<string> detailsInDb, IDbConnection connection, IDbTransaction transaction)
        {
            var detailsToUpdate = detailsInProcess.Where(x => detailsInDb.Contains(x.IP)).ToList();
            //Parallel.ForEach(detailsToUpdate, x => _ipDetailsRepository.Update(x.ToDetailsDTO()));

            await Task.Delay(5000);

            string sql = @"UPDATE IPDetails 
                            SET City = @City, Continent = @Continent, Country = @Country,
                                Latitude = @Latitude, Longitude = @Longitude
                            WHERE IP = @IP";
            Parallel.ForEach(detailsToUpdate, x => connection.ExecuteAsync(sql, x.ToDetailsDTO(), transaction));
        }

        private async Task InsertNotExistingIPDetailRecords(List<IPDetailsModel> detailsInProcess, List<string> detailsInDb, IDbConnection connection, IDbTransaction transaction)
        {
            var detailsToInsert = detailsInProcess.Where(x => !detailsInDb.Contains(x.IP)).ToList();
            //Parallel.ForEach(detailsToInsert, x => _ipDetailsRepository.Insert(x.ToDetailsDTO()));

            await Task.Delay(10000);

            string sql = @"INSERT INTO IPDetails (IP, City, Country, Continent, Latitude, Longitude)
                            VALUES (@IP, @City, @Country, @Continent, @Latitude, @Longitude)";
            Parallel.ForEach(detailsToInsert, x => connection.ExecuteAsync(sql, x.ToDetailsDTO(), transaction));
        }

        private async Task UpdateBatchRecord(Guid batchId, int processedRecords, DateTime? endDate, IDbConnection connection, IDbTransaction transaction)
        {
            //var batchDetail = await _batchDetailsRepository.GetById(batchId);
            //batchDetail.No_of_Updates_Processed = processedRecords;
            //batchDetail.EndTime = endDate;
            //_batchDetailsRepository.Update(batchDetail);

            await Task.Delay(15000);

            string sql = @"UPDATE BatchDetails 
                            SET No_of_Updates_Processed = @No_of_Updates_Processed, EndTime = @EndTime
                            WHERE ID = @ID";
            connection.ExecuteAsync(sql,
                new { No_of_Updates_Processed = processedRecords, EndTime = endDate, ID = batchId }, transaction);
        }

    }
}
