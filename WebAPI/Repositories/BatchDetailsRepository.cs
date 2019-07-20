using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
