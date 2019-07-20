using System;
using System.Collections.Generic;
using System.Text;
using IPInfo.Interfaces;
using IPInfo.Models;
using IPInfo.Models.IPStack;

namespace IPInfo.Common
{
    internal static class Trasformations
    {
        internal static IPDetails ConvertToIPDetails(this IPStackResponseModel resp)
        {
            return new IP_Details
            {
                City = resp.City,
                Continent = resp.ContinentName,
                Country = resp.CountryName,
                Latitude = resp.Latitude,
                Longitude = resp.Longitude
            };
        }
    }
}
