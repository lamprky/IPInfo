using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using WebAPI.Data;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IDbContextTransaction Transaction { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransaction()
        {
            Transaction = await _context.Database.BeginTransactionAsync() ;
        }

        public void CommitTransaction()
        {
            Transaction.Commit();
        }
        public void RollbackTransaction()
        {
            Transaction.Rollback();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
