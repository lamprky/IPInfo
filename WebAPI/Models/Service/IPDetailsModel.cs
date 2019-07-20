using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using IPInfo.Interfaces;

namespace WebAPI.Models.Service
{
    [DataContract]
    public class IPDetailsModel : IPDetails
    {
        [DataMember(Name = "ip")]
        public string IP { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "continent")]
        public string Continent { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }
    }
}
