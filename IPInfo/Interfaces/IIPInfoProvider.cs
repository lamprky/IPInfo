using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IPInfo.Interfaces
{
    public interface IIPInfoProvider
    {
        Task<IPDetails> GetDetails(string ip);
    }
}
