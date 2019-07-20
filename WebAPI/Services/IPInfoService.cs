using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IPInfo;
using IPInfo.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Common;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Models.Service;
using WebAPI.Repositories;
using WebAPI.Repositories.Interfaces;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class IPInfoService : IIPInfoService
    {
        private int cacheExpirationMins = int.Parse(ConfigurationManager.AppSettings.Get("cacheExpirationMins"));

        private IMemoryCache _cache;
        private readonly IUnitOfWork _uow;
        private readonly IIPDetailsRepository _ipDetailsRepository;

        public IPInfoService(IMemoryCache memoryCache,
            IUnitOfWork uow,
            IIPDetailsRepository ipDetailsRepository)
        {
            _cache = memoryCache;
            _uow = uow;
            _ipDetailsRepository = ipDetailsRepository;
        }

        public async Task<IPDetails> GetIPDetails(string ip)
        {
            IPDetails details;
            try
            {
                details = await GetIPDetailsFromCache(ip);
                if (details == null)
                {
                    using (_uow)
                    {
                        details = await GetIPDetailsFromDB(ip);
                        if (details == null)
                        {
                            details = await GetIPDetailsFromIPStack(ip);
                            await SaveInDB(ip, details);
                        }
                    }
                    SaveInCache(ip, details);
                }
            }
            catch
            {
                throw;
            }

            return details;
        }

        private async Task<IPDetails> GetIPDetailsFromCache(string ip)
        {
            if (_cache.TryGetValue(ip, out IPDetails details))
                return details;
            return null;
        }

        private async Task<IPDetails> GetIPDetailsFromDB(string ip)
        {
            var ipDetailsMatch = await _ipDetailsRepository.Get(x => x.IP == ip);
            var details = ipDetailsMatch.FirstOrDefault();
            if (details != null)
                return details.ToDetailsModel();
            else
                return null;
        }

        private async Task<IPDetails> GetIPDetailsFromIPStack(string ip)
        {
            IIPInfoProvider pr = new IPInfoProvider();
            return await pr.GetDetails(ip);
        }

        private void SaveInCache(string ip, IPDetails details)
        {
            if (!_cache.TryGetValue(ip, out IPDetails cacheEntry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cacheExpirationMins));
                _cache.Set(ip, details, cacheEntryOptions);
            }
        }

        private async Task SaveInDB(string ip, IPDetails details)
        {
            IPDetailsDTO det = details.ToDetailsDTO(ip);

            await _ipDetailsRepository.Insert(det);
            await _uow.Save();
        }
    }
}