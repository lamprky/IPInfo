using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Models.Database;

namespace WebAPI.Repositories.Interfaces
{
    public interface IBatchDetailsRepository : IGenericRepository<BatchDetailsDTO>
    {
        Task UpdateDetail(int processedRecords, DateTime? endDate, Guid batchId, IDbConnection connection, IDbTransaction transaction);
    }
}
