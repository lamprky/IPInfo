using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static IPInfo.Common.Exceptions;

namespace IPInfo.Common
{
    public static class Validations
    {
        public static void Validate(this string ip)
        {
            if (string.IsNullOrEmpty(ip))
                throw new IPServiceNotAvailableException("IP is required");

            IPAddress address;
            if (!IPAddress.TryParse(ip, out address))
                throw new IPServiceNotAvailableException("IP is not valid");
        }
    }
}
