using System;
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

namespace WebAPI.Services
{
    public class IPDetailsUpdateBatchService : IIPDetailsUpdateBatchService
    {
        private int cacheExpirationMins = int.Parse(ConfigurationManager.AppSettings.Get("cacheExpirationMins"));
        private int batchRecords = int.Parse(ConfigurationManager.AppSettings.Get("batchRecords"));
        private string strConnString = ConfigurationManager.AppSettings.Get("DefaultConnection");

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

        public async Task<Guid> UpdateIPDetails(List<IPDetailsModel> details)
        {
            Guid batchId = new Guid();
            try
            {
                details.ValidateDetails();

                //using (_uow)
                //{
                //    batchId = await CreateBatchDetail(details.Count);
                //}
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
            using (_uow)
            {
                while (loopsCounter < maxLoops)
                {
                    //transaction = connection.BeginTransaction();

                    int recordsToTake = (loopsCounter + 1 == maxLoops) ? details.Count % batchRecords : batchRecords;
                    var detailsInProcess = details.Skip(loopsCounter * batchRecords).Take(recordsToTake).ToList();

                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

                    try
                    {
                        //find them already exists in db
                        var detailsInDb = await _ipDetailsRepository.Get(x => detailsInProcess.Select(y => y.IP).Contains(x.IP));

                        //update them
                        Task updateRecs = UpdateExistingIPDetailRecords(detailsInProcess, detailsInDb);

                        //for the rest do insert
                        Task insertRecs = InsertNotExistedIPDetailRecords(detailsInProcess, detailsInDb);

                        //update batch record
                        var processedRecords = (loopsCounter * batchRecords) + batchRecords;
                        bool isLastLoop = (loopsCounter + 1 > maxLoops);
                        Task updateBatchRec = UpdateBatchRecord(batchId, processedRecords, isLastLoop);

                        await Task.WhenAll(new List<Task> { updateRecs, insertRecs, updateBatchRec });

                        //transaction.Commit();

                        loopsCounter++;
                    }
                    catch (Exception ex)
                    {
                        //transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private async Task UpdateExistingIPDetailRecords(List<IPDetailsModel> detailsInProcess, List<IPDetailsDTO> detailsInDb)
        {
            //var detailsToUpdate = detailsInProcess.Where(x => detailsInDb.Select(y => y.IP).Contains(x.IP)).ToList();
            //Parallel.ForEach(detailsToUpdate, x => _ipDetailsRepository.Update(x.ToDetailsDTO()));

            await Task.Delay(5000);

            Console.WriteLine("Done 5000");
        }

        private async Task InsertNotExistedIPDetailRecords(List<IPDetailsModel> detailsInProcess, List<IPDetailsDTO> detailsInDb)
        {
            //var detailsToInsert = detailsInProcess.Where(x => !detailsInDb.Select(y => y.IP).Contains(x.IP)).ToList();
            //Parallel.ForEach(detailsToInsert, x => _ipDetailsRepository.Insert(x.ToDetailsDTO()));

            await Task.Delay(10000);

            Console.WriteLine("Done 10000");
        }

        private async Task UpdateBatchRecord(Guid batchId, int processedRecords, bool isLastLoop)
        {
            //var batchDetail = await _batchDetailsRepository.GetById(batchId);
            //batchDetail.No_of_Updates_Processed = processedRecords;
            //batchDetail.EndTime = (isLastLoop) ? DateTime.Now : (DateTime?)null;
            //_batchDetailsRepository.Update(batchDetail);

            await Task.Delay(15000);

            Console.WriteLine("Done 15000");
        }

    }
}
