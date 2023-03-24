using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Entity.Oanda;
using TraderDashboardUi.Entity.Oanda.Order;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.Utilities;
using static TraderDashboardUi.Entity.Oanda.OandaAccountResponse;
using static TraderDashboardUi.Entity.Oanda.OandaCandlesResponse;

namespace TraderDashboardUi.Repository.Providers
{
    public class OandaDataProvider : IOandaDataProvider
    {
        private readonly ILogger<OandaDataProvider> _logger;
        private RestClient _restClient;
        private readonly TraderDashboardConfigurations _traderDashboardConfigurations;
        private readonly PracticeTradeSettings _practiceTradeSettings;
        private Utilites _utilites;

        public OandaDataProvider(ILogger<OandaDataProvider> logger, RestClient restClient, TraderDashboardConfigurations traderDashboardConfigurations, PracticeTradeSettings practiceTradeSettings)
        {
            _logger = logger;
            _restClient = restClient;
            _traderDashboardConfigurations = traderDashboardConfigurations;
            _practiceTradeSettings = practiceTradeSettings;
        }

        public async Task<OandaAccount> GetOandaAccount(string accountType)
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

                if (accountType == "live")
                {
                    _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);
                }
                else if (accountType == "practice")
                {
                    _restClient.Options.BaseUrl = new Uri(oandaSettings.PracticeBaseUrl);
                }


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
                var taskResponse = await Utilites.GetApiResponse(_restClient, request);

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

        public async Task<OandaCandles> GetOandaCandles(string instrument, DateTime backTestStartDate, DateTime backTestEndDate)
        {
            var logInfo = new
            {
                Description = "Get Oanda Candles Method Called -",
                Provider = nameof(OandaDataProvider)
            };

            _logger.LogInformation(JsonConvert.SerializeObject(logInfo));

            _restClient ??= new RestClient();

            try
            {
                var oandaSettings = _traderDashboardConfigurations.oandaSettings;
                var endPoint = oandaSettings.Endpoints.FirstOrDefault(x => x.Key == "candles").Value;

                _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);

                var logOandaCandlesInfo = new
                {
                    Description = "Getting candles from oanda for - ",
                    instrument,
                    backTestStartDate,
                    backTestEndDate,
                    Provider = nameof(OandaDataProvider)
                };

                _logger.LogInformation(JsonConvert.SerializeObject(logOandaCandlesInfo));

                // create a request
                var request = new RestRequest(endPoint, Method.Get);
                request.AddHeader("Authorization", oandaSettings.RequestHeaders.FirstOrDefault(x => x.Key == "Authorization").Value);
                request.AddUrlSegment("accountId", oandaSettings.Id);
                request.AddUrlSegment("instrument", instrument);
                request.AddParameter("from", backTestStartDate);
                request.AddParameter("to", backTestEndDate);
                request.AddParameter("granularity", "M1");

                // get api response
                var taskResponse = await Utilites.GetApiResponse(_restClient, request);

                var result = JsonConvert.DeserializeObject<OandaCandles>(taskResponse);

                logInfo = new
                {
                    Description = "GetOandaCandles Method Ended - ",
                    Provider = nameof(OandaDataProvider)
                };

                _logger.LogInformation(JsonConvert.SerializeObject(logInfo));

                return result;

            }
            catch (Exception ex)
            {
                var logErr = new
                {
                    Description = "Error ocurred in the GetOandaCandles Method",
                    ex.Message,
                    ex.StackTrace,
                    Provider = nameof(OandaDataProvider),
                    Level = "Exception"
                };

                _logger.LogError(JsonConvert.SerializeObject(logErr));

                throw;
            }
        }

        public async Task<OandaCandles> GetOandaLatestCandles(string instrument)
        {
            if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
            {
                var logInfo = new
                {
                    Description = "Get Oanda LatestCandles Method Called -",
                    Provider = nameof(OandaDataProvider)
                };

                _logger.LogInformation(JsonConvert.SerializeObject(logInfo));
            }

            _restClient ??= new RestClient();

            try
            {
                var oandaSettings = _traderDashboardConfigurations.oandaSettings;
                var endPoint = oandaSettings.Endpoints.FirstOrDefault(x => x.Key == "latestCandles").Value;

                _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);

                // create a request
                var request = new RestRequest(endPoint, Method.Get);
                request.AddHeader("Authorization", oandaSettings.RequestHeaders.FirstOrDefault(x => x.Key == "Authorization").Value);
                request.AddUrlSegment("accountId", oandaSettings.Id);
                request.AddParameter("instrument", instrument);
                request.AddParameter("granularity", "M1");

                // get api response
                var taskResponse = await Utilites.GetApiResponse(_restClient, request);

                var result = JsonConvert.DeserializeObject<OandaCandles>(taskResponse);

                return result;


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                throw;
            }
        }

        public async Task<OrderResponse> SendBuyMarketOrder(string instrument)
        {
            var payload = new
            {
                order = new MarketOrder
                {
                    Units = _practiceTradeSettings.Units.ToString(),
                    Instrument = instrument,
                    TimeInForce = "FOK",
                    Type = "MARKET",
                    PositionFill = "DEFAULT"
                }
            };

            _restClient ??= new RestClient();

            try
            {
                var oandaSettings = _traderDashboardConfigurations.oandaSettings;
                var endPoint = oandaSettings.Endpoints.FirstOrDefault(x => x.Key == "createOrder").Value;

                _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);

                // create a request
                var request = new RestRequest(endPoint, Method.Post);
                request.AddHeader("Authorization", oandaSettings.RequestHeaders.FirstOrDefault(x => x.Key == "Authorization").Value);
                request.AddUrlSegment("accountId", oandaSettings.Id);
                string jsonPayload = JsonConvert.SerializeObject(payload);
                request.AddParameter("application/json", jsonPayload, ParameterType.RequestBody);

                // get api response
                var taskResponse = await Utilites.GetApiResponse(_restClient, request);

                var result = JsonConvert.DeserializeObject<OrderResponse>(taskResponse);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                throw;
            }

        }

        public async Task<OrderResponse> SendSellMarketOrder(string instrument)
        {
            var units = -_practiceTradeSettings.Units;
            var payload = new
            {
                order = new MarketOrder
                {
                    Units = units.ToString(),
                    Instrument = instrument,
                    TimeInForce = "FOK",
                    Type = "MARKET",
                    PositionFill = "DEFAULT"
                }
            };

            _restClient ??= new RestClient();

            try
            {
                var oandaSettings = _traderDashboardConfigurations.oandaSettings;
                var endPoint = oandaSettings.Endpoints.FirstOrDefault(x => x.Key == "createOrder").Value;

                _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);

                // create a request
                var request = new RestRequest(endPoint, Method.Post);
                request.AddHeader("Authorization", oandaSettings.RequestHeaders.FirstOrDefault(x => x.Key == "Authorization").Value);
                request.AddUrlSegment("accountId", oandaSettings.Id);
                string jsonPayload = JsonConvert.SerializeObject(payload);
                request.AddParameter("application/json", jsonPayload, ParameterType.RequestBody);

                // get api response
                var taskResponse = await Utilites.GetApiResponse(_restClient, request);

                var result = JsonConvert.DeserializeObject<OrderResponse>(taskResponse);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                throw;
            }

        }

        public async Task<OrderResponse> CloseOrderById(string orderId)
        {
            _restClient ??= new RestClient();

            try
            {
                var oandaSettings = _traderDashboardConfigurations.oandaSettings;
                var endPoint = oandaSettings.Endpoints.FirstOrDefault(x => x.Key == "closeOrderById").Value;

                _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);

                // create a request
                var request = new RestRequest(endPoint, Method.Put);
                request.AddHeader("Authorization", oandaSettings.RequestHeaders.FirstOrDefault(x => x.Key == "Authorization").Value);
                request.AddUrlSegment("accountId", oandaSettings.Id);
                request.AddUrlSegment("tradeId", orderId);


                // get api response
                var taskResponse = await Utilites.GetApiResponse(_restClient, request);

                var result = JsonConvert.DeserializeObject<OrderResponse>(taskResponse);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return null;
        }

        public async Task<OrderResponse> SendBuyLimitOrder(string instrument, string price, string time)
        {
            var payload = new
            {
                order = new LimitOrder
                {
                    Price = price,
                    Units = _practiceTradeSettings.Units.ToString(),
                    Instrument = instrument,
                    TimeInForce = TimeInForce.GTD,
                    OrderType = OrderType.LIMIT,
                    PositionFill = "DEFAULT",
                    GtdTime = DateTime.Parse(time).ToUniversalTime().AddMinutes(5).ToString(),
                }
            };

            _restClient ??= new RestClient();

            try
            {
                var oandaSettings = _traderDashboardConfigurations.oandaSettings;
                var endPoint = oandaSettings.Endpoints.FirstOrDefault(x => x.Key == "createOrder").Value;

                _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);

                // create a request
                var request = new RestRequest(endPoint, Method.Post);
                request.AddHeader("Authorization", oandaSettings.RequestHeaders.FirstOrDefault(x => x.Key == "Authorization").Value);
                request.AddUrlSegment("accountId", oandaSettings.Id);
                string jsonPayload = JsonConvert.SerializeObject(payload);
                request.AddParameter("application/json", jsonPayload, ParameterType.RequestBody);

                // get api response
                var taskResponse = await Utilites.GetApiResponse(_restClient, request);

                var result = JsonConvert.DeserializeObject<OrderResponse>(taskResponse);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return null;
        }

        public async Task<OrderResponse> SendSellLimitOrder(string instrument, string price, string time)
        {
            var payload = new
            {
                order = new LimitOrder
                {
                    Price = price,
                    Units = $"-{_practiceTradeSettings.Units}",
                    Instrument = instrument,
                    TimeInForce = TimeInForce.GTD,
                    OrderType = OrderType.LIMIT,
                    PositionFill = "DEFAULT",
                    GtdTime = DateTime.Parse(time).ToUniversalTime().AddMinutes(5).ToString(),
                }
            };

            _restClient ??= new RestClient();

            try
            {
                var oandaSettings = _traderDashboardConfigurations.oandaSettings;
                var endPoint = oandaSettings.Endpoints.FirstOrDefault(x => x.Key == "createOrder").Value;

                _restClient.Options.BaseUrl = new Uri(oandaSettings.BaseUrl);

                // create a request
                var request = new RestRequest(endPoint, Method.Post);
                request.AddHeader("Authorization", oandaSettings.RequestHeaders.FirstOrDefault(x => x.Key == "Authorization").Value);
                request.AddUrlSegment("accountId", oandaSettings.Id);
                string jsonPayload = JsonConvert.SerializeObject(payload);
                request.AddParameter("application/json", jsonPayload, ParameterType.RequestBody);

                // get api response
                var taskResponse = await Utilites.GetApiResponse(_restClient, request);

                var result = JsonConvert.DeserializeObject<OrderResponse>(taskResponse);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return null;
        }
    }
}
