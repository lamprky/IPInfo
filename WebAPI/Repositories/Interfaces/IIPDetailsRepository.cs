using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Repositories.Interfaces
{
    public interface IIPDetailsRepository : IGenericRepository<IPDetailsDTO>
    {
        List<IPDetailsDTO> GetByIps(List<string> detailsInProcess, IDbConnection connection, IDbTransaction transaction);

        void UpdateDetail(IPDetailsDTO detail, IDbConnection connection, IDbTransaction transaction);
        void InsertDetail(IPDetailsDTO detail, IDbConnection connection, IDbTransaction transaction);

    }
}
