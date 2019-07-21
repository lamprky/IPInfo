using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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

        private readonly string select_sql = "SELECT * FROM IPDetails WHERE IP IN @ips";

        private readonly string insert_sql = 
            @"INSERT INTO IPDetails (IP, City, Country, Continent, Latitude, Longitude)
                VALUES (@IP, @City, @Country, @Continent, @Latitude, @Longitude)";

        private readonly string update_sql =
            @"UPDATE IPDetails 
                SET City = @City, Continent = @Continent, Country = @Country,
                    Latitude = @Latitude, Longitude = @Longitude
                WHERE IP = @IP";

        public List<IPDetailsDTO> GetByIps(List<string> detailsInProcess, IDbConnection connection, IDbTransaction transaction)
        {
            List<IPDetailsDTO> detailsInDb = connection.Query<IPDetailsDTO>(select_sql, new { ips = detailsInProcess.ToArray() }, transaction).ToList();
            return detailsInDb;
        }

        public void UpdateDetail(IPDetailsDTO detail, IDbConnection connection, IDbTransaction transaction)
        {
            connection.ExecuteAsync(update_sql, detail, transaction);
        }

        public void InsertDetail(IPDetailsDTO detail, IDbConnection connection, IDbTransaction transaction)
        {
            connection.ExecuteAsync(insert_sql, detail, transaction);
        }
    }
}
