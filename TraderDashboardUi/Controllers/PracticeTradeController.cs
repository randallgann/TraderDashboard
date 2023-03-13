using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TraderDashboardUi.Models;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.Providers;
using TraderDashboardUi.Repository.Utilities;

namespace TraderDashboardUi.Controllers
{
    public class PracticeTradeController : Controller
    {

        private readonly ILogger<PracticeTradeController> _logger;
        private readonly IOandaDataProvider _provider;
        private IBackTestStrategy _backTestStrategy;
        private readonly TraderDashboardConfigurations _traderDashboardConfigurations;
        private readonly ITradeManager _tradeManager;
        private static PracticeTradeThreadRunner _practiceTradeThreadRunner;

        public PracticeTradeController(ILogger<PracticeTradeController> logger, IOandaDataProvider provider, TraderDashboardConfigurations configurations, ITradeManager tradeManager)
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

        [HttpGet]
        public JsonResult GetElapsedTime()
        {
            if (_practiceTradeThreadRunner != null)
            {
                return Json(_practiceTradeThreadRunner._elapsedTime);
            }
            else
            {
                return Json("Unable to access elapsedTime");
            }
        }

        public async Task<IActionResult> StartPracticeTrading(PracticeTradeViewModel model)
        {
            Debug.WriteLine("StartPracticeTrading has been entered");

            _practiceTradeThreadRunner = new PracticeTradeThreadRunner();
            _practiceTradeThreadRunner._isRunning = true;
            model.isRunning = _practiceTradeThreadRunner._isRunning;

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
