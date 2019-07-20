using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPInfo.Interfaces;

namespace WebAPI.Services.Interfaces
{
    public interface IIPInfoService
    {
        Task<IPDetails> GetIPDetails(string ip);
    }
}
