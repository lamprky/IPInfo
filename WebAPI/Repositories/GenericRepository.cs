using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebAPI.Data;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal DbSet<TEntity> dbSet;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            this.dbSet = dbContext.Set<TEntity>();
        }

        public async Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity> GetById(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<EntityEntry<TEntity>> Insert(TEntity entity)
        {
            return await dbSet.AddAsync(entity);
        }

        public void Update(TEntity entityToUpdate)
        {
            dbSet.Update(entityToUpdate);
        }

        public void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            dbSet.Remove(entityToDelete);
        }
    }
}
