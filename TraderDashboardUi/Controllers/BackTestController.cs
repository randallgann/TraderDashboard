using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Models;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.StrategyProcessors;
using TraderDashboardUi.Repository.Utilities;
using TraderDashboardUi.ViewModels;
using static TraderDashboardUi.Models.BackTestResponseViewModel;

namespace TraderDashboardUi.Controllers
{
    public class BackTestController : Controller
    {
        private readonly ILogger<BackTestController> _logger;
        private readonly IOandaDataProvider _provider;
        private IBackTestStrategy _backTestStrategy;
        private readonly TraderDashboardConfigurations _traderDashboardConfigurations;
        private readonly ITradeManager _tradeManager;

        public BackTestController(ILogger<BackTestController> logger, IOandaDataProvider provider, TraderDashboardConfigurations configurations, ITradeManager tradeManager)
        {
            _logger = logger;
            _provider = provider;
            _traderDashboardConfigurations = configurations;
            _tradeManager = tradeManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = GetInitialModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BackTestViewModel model)
        {
            // add error handling for when candles count is too large
            try
            {
                return await ProcessBackTest(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);

                return PartialView("Error", model);
            }
        }

        private async Task<IActionResult> ProcessBackTest(BackTestViewModel model)
        {
            // get the candles
            var candles = await _provider.GetOandaCandles(model.Instrument, model.BackTestStartDate, model.BackTestEndDate);

            // convert to datatable
            DataTable dt = Utilites.ConvertOandaCandlesToDataTable(candles);

            // get appropriate IBeckTestStrategy - StrategyProcessors
            var strategyProcessor = GetStrategyProcessor(model.Strategy);
            if (strategyProcessor == null)
            {
                ModelState.AddModelError(string.Empty, "Strategy Processor is null");
                // ToDo: fix return of error message
                return View(model);
            }
            _backTestStrategy = strategyProcessor;


            // execute backtest
            DataTable backTestResults = _backTestStrategy.ExecuteBackTest(dt);

            // execute trades
            TradeBook tradeResults = _tradeManager.BackTestExecuteTrades(backTestResults);

            BackTestResponseViewModel backTestResponseViewModel = GetBackTestResponseViewModel(backTestResults, tradeResults, model.Strategy);

            //foreach (DataRow row in dt.Rows)
            //{
            //    Candle c = new Candle();
            //    c.Instrument = Convert.ToString(row["Instrument"]);
            //    c.Granularity = Convert.ToString(row["Granularity"]);
            //    c.Complete = Convert.ToString(row["Complete"]);
            //    c.volume = Convert.ToString(row["Volume"]);
            //    c.time = Convert.ToString(row["Time"]);
            //    c.open = Convert.ToString(row["Open"]);
            //    c.high = Convert.ToString(row["High"]);
            //    c.low = Convert.ToString(row["Low"]);
            //    c.close = Convert.ToString(row["Close"]);
            //    backTestResponseViewModel.Candles.Add(c);

            //}



            return await Task.FromResult(PartialView("_BackTestResponse", backTestResponseViewModel));
        }

        private BackTestResponseViewModel GetBackTestResponseViewModel(DataTable backTestResults, TradeBook tradeResults, string strategy)
        {
            BackTestResponseViewModel model = new BackTestResponseViewModel();
            List<Dictionary<string, object>> backTestResultsDictionary = Utilites.ConvertBackTestResultsToDictionary(backTestResults, strategy);
            BackTestResultStats backTestResultStats = Utilites.CalculateBackTestStats(tradeResults);
            model.Stats = backTestResultStats;
            model.CandlesPlusBackTest = backTestResultsDictionary;
            model.PositionsList = tradeResults;
            return model;
        }

        private IBackTestStrategy GetStrategyProcessor(string strategy)
        {
            switch (strategy)
            {
                case "GUPPYMMA":
                    return new BackTestStrategyGuppyMMA();
                default:
                    return null;
            }
        }

        private BackTestModel GetInitialModel()
        {
            var model = new BackTestModel
            {
                Instruments = GetBackTestInstruments(),
                Strategies = GetBackTestStrategies(),
            };

            return model;
        }

        private List<SelectListItem> GetBackTestStrategies()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "--Select--", Value = String.Empty},
                new SelectListItem { Text = TradingStrategies.GUPPYMMA, Value = TradingStrategies.GUPPYMMA},
            };
        }

        private List<SelectListItem> GetBackTestInstruments()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "--Select--", Value = String.Empty},
                new SelectListItem { Text = OandaBackTestCurrencyPairs.CAD_CHF, Value = OandaBackTestCurrencyPairs.CAD_CHF },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.CAD_JPY, Value = OandaBackTestCurrencyPairs.CAD_JPY },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.AUD_CAD, Value = OandaBackTestCurrencyPairs.AUD_CAD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.AUD_CHF, Value = OandaBackTestCurrencyPairs.AUD_CHF },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.AUD_JPY, Value = OandaBackTestCurrencyPairs.AUD_JPY },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.AUD_NZD, Value = OandaBackTestCurrencyPairs.AUD_NZD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.AUD_USD, Value = OandaBackTestCurrencyPairs.AUD_USD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.CHF_JPY, Value = OandaBackTestCurrencyPairs.CHF_JPY },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.EUR_AUD, Value = OandaBackTestCurrencyPairs.EUR_AUD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.EUR_CAD, Value = OandaBackTestCurrencyPairs.EUR_CAD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.EUR_CHF, Value = OandaBackTestCurrencyPairs.EUR_CHF },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.EUR_JPY, Value = OandaBackTestCurrencyPairs.EUR_JPY },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.EUR_NZD, Value = OandaBackTestCurrencyPairs.EUR_NZD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.EUR_USD, Value = OandaBackTestCurrencyPairs.EUR_USD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.GBP_AUD, Value = OandaBackTestCurrencyPairs.GBP_AUD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.GBP_CAD, Value = OandaBackTestCurrencyPairs.GBP_CAD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.GBP_CHF, Value = OandaBackTestCurrencyPairs.GBP_CHF },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.GBP_JPY, Value = OandaBackTestCurrencyPairs.GBP_JPY },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.GBP_NZD, Value = OandaBackTestCurrencyPairs.GBP_NZD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.GBP_USD, Value = OandaBackTestCurrencyPairs.GBP_USD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.NZD_CAD, Value = OandaBackTestCurrencyPairs.NZD_CAD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.NZD_CHF, Value = OandaBackTestCurrencyPairs.NZD_CHF },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.NZD_JPY, Value = OandaBackTestCurrencyPairs.NZD_JPY },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.NZD_USD, Value = OandaBackTestCurrencyPairs.NZD_USD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.USD_CAD, Value = OandaBackTestCurrencyPairs.USD_CAD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.USD_CHF, Value = OandaBackTestCurrencyPairs.USD_CHF },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.USD_JPY, Value = OandaBackTestCurrencyPairs.USD_JPY },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.USD_MXN, Value = OandaBackTestCurrencyPairs.USD_MXN },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.USD_SGD, Value = OandaBackTestCurrencyPairs.USD_SGD },
                new SelectListItem { Text = OandaBackTestCurrencyPairs.USD_ZAR, Value = OandaBackTestCurrencyPairs.USD_ZAR },
            };

        }
    }
}
