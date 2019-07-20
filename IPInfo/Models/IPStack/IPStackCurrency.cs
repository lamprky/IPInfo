using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IPInfo.Models.IPStack
{
    [DataContract]
    internal class IPStackCurrency
    {
        [DataMember(Name = "code")]
        internal string Code { get; set; }

        [DataMember(Name = "name")]
        internal string Name { get; set; }

        [DataMember(Name = "plural")]
        internal string Plural { get; set; }

        [DataMember(Name = "symbol")]
        internal string Symbol { get; set; }

        [DataMember(Name = "symbol_native")]
        internal string SymbolNative { get; set; }
    }
}
