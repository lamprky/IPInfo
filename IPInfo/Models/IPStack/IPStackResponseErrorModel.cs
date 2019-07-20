using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IPInfo.Models.IPStack
{
    [DataContract]
    internal class IPStackResponseErrorModel
    {
        [DataMember(Name = "success")]
        internal bool? Success { get; set; }

        [DataMember(Name = "error")]
        internal Error Error { get; set; }
    }

    [DataContract]
    internal class Error
    {
        [DataMember(Name = "code")]
        internal string Code { get; set; }

        [DataMember(Name = "type")]
        internal string Type { get; set; }

        [DataMember(Name = "info")]
        internal string Info { get; set; }
    }
}
