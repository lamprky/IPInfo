using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace IPInfo.Models.IPStack
{
    [DataContract]
    internal class IPStackResponseModel
    {
        //[DataMember(Name = "ip")]
        //internal string IP { get; set; }

        //[DataMember(Name = "hostname")]
        //internal string Hostname { get; set; }

        //[DataMember(Name = "type")]
        //internal string Type { get; set; }

        //[DataMember(Name = "continent_code")]
        //internal string ContinentCode { get; set; }

        [DataMember(Name = "continent_name")]
        internal string ContinentName { get; set; }

        //[DataMember(Name = "country_code")]
        //internal string CountryCode { get; set; }

        [DataMember(Name = "country_name")]
        internal string CountryName { get; set; }

        //[DataMember(Name = "region_code")]
        //internal string RegionCode { get; set; }

        //[DataMember(Name = "region_name")]
        //internal string RegionName { get; set; }

        [DataMember(Name = "city")]
        internal string City { get; set; }

        //[DataMember(Name = "zip")]
        //internal string Zip { get; set; }

        [DataMember(Name = "latitude")]
        internal double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        internal double Longitude { get; set; }


        //[DataMember(Name = "location")]
        //internal IPStackLocation Location { get; set; } = new IPStackLocation();

        //[DataMember(Name = "time_zone")]
        //internal IPStackTimeZone TimeZone { get; set; } = new IPStackTimeZone();

        //[DataMember(Name = "currency")]
        //internal IPStackCurrency Currency { get; set; } = new IPStackCurrency();

        //[DataMember(Name = "connection")]
        //internal IPStackConnection Connection { get; set; } = new IPStackConnection();

        //[DataMember(Name = "security")]
        //internal IPStackSecurity Security { get; set; } = new IPStackSecurity();

    }
}
