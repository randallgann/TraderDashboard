using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Models;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.Providers;
using TraderDashboardUi.Repository.Utilities;
using static TraderDashboardUi.Entity.Oanda.OandaCandlesResponse;

namespace TraderDashboardUi.Controllers
{
    public class PracticeTradeController : Controller
    {

        private readonly ILogger<PracticeTradeController> _logger;
        private IBackTestStrategy _backTestStrategy;
        private readonly IOandaDataProvider _provider;
        private readonly TraderDashboardConfigurations _traderDashboardConfigurations;
        private readonly ITradeManager _tradeManager;
        private readonly IServiceProvider _serviceProvider;
        private static PracticeTradeThreadRunner _practiceTradeThreadRunner;
        public PracticeTradeSettings _practiceTradeSettings;

        public PracticeTradeController(ILogger<PracticeTradeController> logger, TraderDashboardConfigurations configurations, ITradeManager tradeManager, IOandaDataProvider provider, PracticeTradeSettings practiceTradeSettings, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _traderDashboardConfigurations = configurations;
            _tradeManager = tradeManager;
            _provider = provider;
            _practiceTradeSettings = practiceTradeSettings;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IActionResult Index()
        {

            var model = GetInitialModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PracticeTradeViewModel model)
        {
            // add error handling for when candles count is too large
            try
            {
                return await StartPracticeTrading(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);

                return PartialView("Error", model);
            }
        }

        [AjaxOnly]
        [HttpGet]
        public JsonResult UpdatePracticeTradeRunning()
        {
            if (_practiceTradeThreadRunner != null)
            {
                TimeSpan elapsedTime = _practiceTradeThreadRunner.elapsedTime;
                OCandle mostRecentCandle = _practiceTradeThreadRunner.mostRecentCandle;
                OCandle inProgressCandle = _practiceTradeThreadRunner.inProgressCandle;

                var jsonData = new
                {
                    elapsedTime = elapsedTime.ToString(),
                    candleTime = mostRecentCandle.time,
                    candleVolume = mostRecentCandle.volume,
                    candleOpen = mostRecentCandle.mid.o,
                    candleHigh = mostRecentCandle.mid.h,
                    candleLow = mostRecentCandle.mid.l,
                    candleClose = mostRecentCandle.mid.c,
                    candleComplete = mostRecentCandle.complete,
                    inProgressCandleTime = inProgressCandle.time,
                    inProgressCandleVolume = inProgressCandle.volume,
                    inProgressCandleOpen = inProgressCandle.mid.o,
                    inProgressCandleHigh = inProgressCandle.mid.h,
                    inProgressCandleLow = inProgressCandle.mid.l,
                    inProgressCandleClose = inProgressCandle.mid.c,
                    inProgressCandleComplete = inProgressCandle.complete,
                    candleCounter = _practiceTradeThreadRunner._tradeManager.CandleCounter,
                    activeTrades = _practiceTradeThreadRunner._tradeManager.tradeBook.Positions.Count(p => p.ActiveTrade),
                    realizedPL = _practiceTradeThreadRunner._tradeManager.tradeBook.Positions.Sum(p => p.ActiveTradeRealizedPL),
                    numberOfTotalTrades = _practiceTradeThreadRunner._tradeManager.tradeBook.Positions.Count,
                };

                return Json(jsonData);
            }
            else
            {
                return Json("Unable to access elapsedTime");
            }
        }

        [AjaxOnly]
        [HttpGet]
        public JsonResult UpdateActiveTradeInfo()
        {
            if (_practiceTradeThreadRunner != null)
            {
                var _currentTradeStopLoss = "";
                var _currentTradeTakeProfit = "";
                if (_practiceTradeThreadRunner._tradeManager.tradeBook.Positions.Count(p => p.ActiveTrade) > 0)
                {
                    var entryPrice = _practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).Price;
                    var stopLoss = _practiceTradeThreadRunner._tradeManager.PipStopLoss;
                    var takeProfit = _practiceTradeThreadRunner._tradeManager.PipTakeProfit;
                    if (_practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).ActiveTradeBuySell == "Buy")
                    {
                        _currentTradeTakeProfit = $"{entryPrice} + {takeProfit} = {_practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).ActiveTradePipTakeProfit}";
                        _currentTradeStopLoss = $"{entryPrice} - {stopLoss} = {_practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).ActiveTradePipStopLoss}";
                    }
                    else
                    {
                        _currentTradeTakeProfit = $"{entryPrice} - {takeProfit} = {_practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).ActiveTradePipTakeProfit}";
                        _currentTradeStopLoss = $"{entryPrice} + {stopLoss} = {_practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).ActiveTradePipStopLoss}";
                    }
                    var jsonData = new
                    {
                        currentTradeEntryPrice = _practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).Price,
                        currentTradeStopLoss = _currentTradeStopLoss,
                        currentTradeTakeProfit = _currentTradeTakeProfit,
                        currentTradeEntryTime = _practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).Time,
                        currentTradeEntryDirection = _practiceTradeThreadRunner._tradeManager.tradeBook.Positions.FirstOrDefault(p => p.ActiveTrade).ActiveTradeBuySell,
                    };
                                    
                    return Json(jsonData);
                }
                else
                {
                    return Json("No Active Trades");
                }
            }
            else
            {
                return Json("PracticeTradeThreadRunner is Null");
            }
        }

        [AjaxOnly]
        [HttpGet]
        public IActionResult UpdatePracticeTradeDataTable()
        {
            DataTable dt = _practiceTradeThreadRunner.tradingDataTable;
            IEnumerable<DataRow> rows = dt.AsEnumerable().Reverse().Take(10);
            DataTable last10Rows = rows.CopyToDataTable();
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(last10Rows);
            return Json(JSONString);
        }

        public async Task<IActionResult> StartPracticeTrading(PracticeTradeViewModel model)
        {
            //_practiceTradeThreadRunner = new PracticeTradeThreadRunner(model.Instrument, model.Strategy, _traderDashboardConfigurations, _practiceTradeSettings, _tradeManager);
            _practiceTradeThreadRunner = _serviceProvider.GetService<PracticeTradeThreadRunner>();
            _practiceTradeThreadRunner.instrument = model.Instrument;
            _practiceTradeThreadRunner.strategy = model.Strategy;
            _practiceTradeThreadRunner.StartThread();
            _practiceTradeThreadRunner.isRunning = true;
            model.isRunning = _practiceTradeThreadRunner.isRunning;

            return await Task.FromResult(PartialView("_PracticeTradeRunning", model));
        }

        [AjaxOnly]
        [HttpPost]
        public async Task<IActionResult> StopRunningPracticeTrade(PracticeTradeViewModel model)
        {
            _practiceTradeThreadRunner.Dispose();
            model.isRunning = false;
            model.elapsedTime = 0;
            return await Task.FromResult(PartialView("_PracticeTradeRunning", model));

        }

        private PracticeTradeModel GetInitialModel()
        {
            var model = new PracticeTradeModel
            {
                Instruments = Utilites.GetBackTestInstruments(),
                Strategies = Utilites.GetBackTestStrategies(),
            };

            return model;
        }
    }
}
