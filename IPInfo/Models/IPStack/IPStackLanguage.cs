using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IPInfo.Models.IPStack
{
    [DataContract]
    internal class IPStackLanguage
    {
        [DataMember(Name = "code")]
        internal string Code { get; set; }

        [DataMember(Name = "name")]
        internal string Name { get; set; }

        [DataMember(Name = "native")]
        internal string Native { get; set; }
    }
}
