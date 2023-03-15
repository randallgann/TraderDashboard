using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
        private static PracticeTradeThreadRunner _practiceTradeThreadRunner;

        public PracticeTradeController(ILogger<PracticeTradeController> logger, TraderDashboardConfigurations configurations, ITradeManager tradeManager, IOandaDataProvider provider)
        {
            _logger = logger;
            _traderDashboardConfigurations = configurations;
            _tradeManager = tradeManager;
            _provider = provider;
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
                };

                return Json(jsonData);
            }
            else
            {
                return Json("Unable to access elapsedTime");
            }
        }

        public async Task<IActionResult> StartPracticeTrading(PracticeTradeViewModel model)
        {
            _practiceTradeThreadRunner = new PracticeTradeThreadRunner(model.Instrument, model.Strategy, _traderDashboardConfigurations);
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
