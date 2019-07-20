using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Runtime.Serialization;

namespace IPInfo.Models.IPStack
{
    [DataContract]
    internal class IPStackLocation
    {
        [DataMember(Name = "geoname_id")]
        internal long? GeonameId { get; set; }

        [DataMember(Name = "capital")]
        internal string Capital { get; set; }

        [DataMember(Name = "languages")]
        internal List<IPStackLanguage> Languages { get; set; } = new List<IPStackLanguage>();

        [DataMember(Name = "country_flag")]
        internal string CountryFlag { get; set; }

        [DataMember(Name = "country_flag_emoji")]
        internal string CountryFlagEmoji { get; set; }

        [DataMember(Name = "country_flag_emoji_unicode")]
        internal string CountryFlagEmojiUnicode { get; set; }

        [DataMember(Name = "calling_code")]
        internal string CallingCode { get; set; }

        [DataMember(Name = "is_eu")]
        internal bool? IsEu { get; set; }
    }
}
