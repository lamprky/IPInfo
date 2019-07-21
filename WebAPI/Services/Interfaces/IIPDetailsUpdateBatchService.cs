using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Service;

namespace WebAPI.Services.Interfaces
{
    public interface IIPDetailsUpdateBatchService
    {
        Task<Guid> UpdateIPDetails(List<IPDetailsModel> details);
        Task<BatchDetailModel> GetJobProgress(Guid batchId);
    }
}
