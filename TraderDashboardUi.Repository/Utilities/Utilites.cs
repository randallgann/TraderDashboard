using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TraderDashboardUi.Repository.Utilities
{
    public class Utilites
    {
        public static async Task<string> GetApiResponse(RestClient restClient, RestRequest request)
        {
            var response = await restClient.ExecuteAsync(request);
            return response.Content;
        }
        public static void AddRequestHeaders(Dictionary<string, string> requestHeaders, HttpClient httpClient, string token = null)
        {
            foreach (var (key, keyValue) in requestHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(key, keyValue);
            }

            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
