using System;
using System.Collections.Generic;
using System.Text;
using IPInfo.Interfaces;

namespace IPInfo.Models
{
    public class IP_Details : IPDetails
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
