using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TraderDashboardUi.Entity.Oanda;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.Utilities;
using static TraderDashboardUi.Entity.Oanda.OandaAccountResponse;

namespace TraderDashboardUi.Repository.Providers
{
    public class OandaDataProvider : IOandaDataProvider
    {
        private readonly ILogger<OandaDataProvider> _logger;
        private RestClient _restClient;
        private readonly TraderDashboardConfigurations _traderDashboardConfigurations;

        public OandaDataProvider(ILogger<OandaDataProvider> logger, RestClient restClient, TraderDashboardConfigurations traderDashboardConfigurations)
        {
            _logger = logger;
            _restClient = restClient;
            _traderDashboardConfigurations = traderDashboardConfigurations;
        }

        public async Task<OandaAccount> GetOandaAccount()
        {
            var logInfo = new
            {
                Description = "Get Oanda Account Method Called -",
                Provider = nameof(OandaDataProvider)
            };

            _logger.LogInformation(JsonConvert.SerializeObject(logInfo));

            // this creates the httpclient only if it is null; saves resources by reusing the client if it has already been created
            _restClient ??= new RestClient();

            try
            {
                var oandaSettings = _traderDashboardConfigurations.oandaSettings;
                var endPoint = oandaSettings.Endpoints.FirstOrDefault(x => x.Key == "account").Value;
                var oandaAccountResponse = new OandaAccountResponse();

                _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);

                var logOandaSettingsInfo = new
                {
                    Description = "The Oanda Service Settings Values are - ",
                    oandaSettings.BaseUrl,
                    oandaSettings.Id,
                    endPoint,
                    Provider = nameof(OandaDataProvider)
                };

                _logger.LogInformation(JsonConvert.SerializeObject(logOandaSettingsInfo));

                // create a request
                var request = new RestRequest(endPoint, Method.Get);
                request.AddUrlSegment("accountId", oandaSettings.Id);
                request.AddHeader("Authorization", oandaSettings.RequestHeaders.FirstOrDefault(x => x.Key == "Authorization").Value);

                // get api response
                var taskResponse = Utilites.GetApiResponse(_restClient, request).Result;

                var result = JsonConvert.DeserializeObject<OandaAccount>(taskResponse);

                logInfo = new
                {
                    Description = "GetOandaAccount Method Ended - ",
                    Provider = nameof(OandaDataProvider)
                };
                
                _logger.LogInformation(JsonConvert.SerializeObject(logInfo));

                return result;

            }
            catch (Exception ex)
            {
                var logErr = new
                {
                    Description = "Error ocurred in the GetOandaAccount Method",
                    ex.Message,
                    ex.StackTrace,
                    Provider = nameof(OandaDataProvider),
                    Level = "Exception"
                };

                _logger.LogError(JsonConvert.SerializeObject(logErr));

                return null;
            }
        }
    }
}
