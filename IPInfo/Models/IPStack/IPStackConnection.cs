using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IPInfo.Models.IPStack
{
    [DataContract]
    internal class IPStackConnection
    {
        [DataMember(Name = "asn")]
        public int? Asn { get; set; }

        [DataMember(Name = "isp")]
        public string Isp { get; set; }
    }
}
