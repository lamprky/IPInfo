using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Service;

namespace WebAPI.Common
{
    public static class Validations
    {
        public static void ValidateDetails(this List<IPDetailsModel> details)
        {
            details.ForEach(x=>x.ValidateDetail());
        }

        public static void ValidateDetail(this IPDetailsModel detail)
        {
            if (string.IsNullOrEmpty(detail.IP))
                throw new MissingFieldException("IP cannot be null or empty");

            //TODO: anything alse to ckeck?
        }
    }
}
