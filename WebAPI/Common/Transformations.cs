using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPInfo.Interfaces;
using WebAPI.Models;
using WebAPI.Models.Database;
using WebAPI.Models.Service;

namespace WebAPI.Common
{
    public static class Transformations
    {
        public static IPDetailsModel ToDetailsModel(this IPDetailsDTO dbObj)
        {
            IPDetailsModel details = new IPDetailsModel
            {
                IP = dbObj.IP,
                City = dbObj.City,
                Continent = dbObj.Continent,
                Country = dbObj.Country,
                Latitude = dbObj.Latitude,
                Longitude = dbObj.Longitude
            };

            return details;
        }

        public static IPDetailsDTO ToDetailsDTO(this IPDetails model, string ip)
        {
            IPDetailsDTO details = new IPDetailsDTO
            {
                IP = ip,
                City = model.City,
                Continent = model.Continent,
                Country = model.Country,
                Latitude = model.Latitude,
                Longitude = model.Longitude
            };

            return details;
        }

        public static IPDetailsDTO ToDetailsDTO(this IPDetailsModel model)
        {
            return model.ToDetailsDTO(model.IP);
        }

        public static BatchDetailModel ToBatchDetailModel(this BatchDetailsDTO model)
        {
            return new BatchDetailModel {
                Progress = model.No_of_Updates_Processed + "/" + model.No_of_Updates,
                StartDate = model.StartTime,
                EndDate = model.EndTime,
            };
        }
    }
}
