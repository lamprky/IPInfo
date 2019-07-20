using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace IPInfo.Models.IPStack
{
    [DataContract]
    internal class IPStackSecurity
    {
        [DataMember(Name = "is_proxy")]
        internal bool? IsProxy { get; set; }

        [DataMember(Name = "proxy_type")]
        internal string ProxyType { get; set; }

        [DataMember(Name = "is_crawler")]
        internal bool? IsCrawler { get; set; }

        [DataMember(Name = "crawler_name")]
        internal string CrawlerName { get; set; }

        [DataMember(Name = "crawler_type")]
        internal string CrawlerType { get; set; }

        [DataMember(Name = "is_tor")]
        internal bool? IsTor { get; set; }

        [DataMember(Name = "threat_level")]
        internal string ThreatLevel { get; set; }

        [DataMember(Name = "threat_types")]
        internal List<string> ThreatTypes { get; set; } = new List<string>();
    }
}
