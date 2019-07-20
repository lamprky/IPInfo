using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebAPI.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
        Task<TEntity> GetById(object id);
        Task<EntityEntry<TEntity>> Insert(TEntity obj);
        Task Update(TEntity obj);
        Task Delete(object id);
        Task Delete(TEntity obj);
    }
}
