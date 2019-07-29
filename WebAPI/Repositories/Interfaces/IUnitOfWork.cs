using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable //Provides a mechanism for releasing unmanaged resources.
    {
        Task<int> Save();
        Task BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
