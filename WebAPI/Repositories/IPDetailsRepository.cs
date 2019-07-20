using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories
{
    public class IPDetailsRepository: GenericRepository<IPDetailsDTO>, IIPDetailsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public IPDetailsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
