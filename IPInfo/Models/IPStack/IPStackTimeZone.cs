using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace IPInfo.Models.IPStack
{
    [DataContract]
    internal class IPStackTimeZone
    {
        [DataMember(Name = "id")]
        internal string id { get; set; }

        [DataMember(Name = "current_time")]
        internal DateTime? CurrentTime { get; set; }

        [DataMember(Name = "gmt_offset")]
        internal int? GmtOffset { get; set; }

        [DataMember(Name = "code")]
        internal string Code { get; set; }

        [DataMember(Name = "is_daylight_saving")]
        internal bool? IsDaylightSaving { get; set; }
    }
}
