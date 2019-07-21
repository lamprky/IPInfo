using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Models.Database;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories
{
    public class BatchDetailsRepository: GenericRepository<BatchDetailsDTO>, IBatchDetailsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BatchDetailsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        private readonly string SQL_UpdateById = 
            @"UPDATE BatchDetails 
                SET No_of_Updates_Processed = @No_of_Updates_Processed, EndTime = @EndTime
                WHERE ID = @ID";
        public async Task UpdateDetail(int processedRecords, DateTime? endDate, Guid batchId, IDbConnection connection, IDbTransaction transaction)
        { 
            await connection.ExecuteAsync(SQL_UpdateById,
                new { No_of_Updates_Processed = processedRecords, EndTime = endDate, ID = batchId }, transaction);
        }
    }
}
