using System;
using System.Threading.Tasks;
using IPInfo.Interfaces;

namespace IPInfo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IIPInfoProvider pr = new IPInfoProvider();
            IPDetails det = await pr.GetDetails("2.86.114.25");
        }
    }
}
