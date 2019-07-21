using System;
using System.Collections.Generic;
using System.Text;
using IPInfo.Common;
using IPInfo.Interfaces;
using IPInfo.Models.IPStack;
using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using System.Collections.Specialized;
using static IPInfo.Common.Exceptions;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace IPInfo
{
    public class IPInfoProvider : IIPInfoProvider
    {
        public async Task<IPDetails> GetDetails(string ip)
        {
            ip.Validate();

            IPDetails details;
            try
            {
                IRestResponse restResp = await GetDetailsFromIpStack(ip);
                IsAnticipatedResponse(restResp);
                IPStackResponseModel resp = DeserializeResponse<IPStackResponseModel>(restResp);
                details = resp.ConvertToIPDetails();
            }
            catch (Exception ex)
            {
                throw new IPServiceNotAvailableException(ex.Message, ex.InnerException);
            }

            return details;
        }
        

        private async Task<IRestResponse> GetDetailsFromIpStack(string ip)
        {
            string url = ConfigurationManager.AppSettings.Get("Url");
            string access_key = ConfigurationManager.AppSettings.Get("AccessKey");
            string response_language = ConfigurationManager.AppSettings.Get("ResponseLanguage"); ;

            var client = new RestClient(url);
            var request = new RestRequest(ip, Method.GET);
            request.AddParameter("access_key", access_key);
            request.AddParameter("output", "json");
            request.AddParameter("fields", GetRequestFields());
            request.AddParameter("language", response_language);

            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token); ;

            return response;
        }

        private string GetRequestFields()
        {
            return "continent_name,country_name,city,latitude,longitude";
        }

        private void IsAnticipatedResponse(IRestResponse restResp)
        {
            if (restResp.StatusCode != System.Net.HttpStatusCode.OK)
                throw new IPServiceNotAvailableException("Service not found");

            // check if error response
            IPStackResponseErrorModel respError = DeserializeResponse<IPStackResponseErrorModel>(restResp);
            if (respError.Success != null)
                throw new IPServiceNotAvailableException(respError.Error.Info);
        }

        private T DeserializeResponse<T>(IRestResponse response) where T : class
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
