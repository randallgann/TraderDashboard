using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TraderDashboardUi.Models;
using TraderDashboardUi.Repository.Interfaces;
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

        private async Task<IActionResult> StartPracticeTrading(PracticeTradeViewModel model)
        {
            model.isRunning = true;

            new Thread(() => {
                while (model.isRunning)
                {
                    model.elapsedTime++;
                }
            }).Start();

            PracticeTradeViewModel practiceTradeViewModel = new PracticeTradeViewModel();

            return await Task.FromResult(PartialView("_PracticeTradeRunning", practiceTradeViewModel));
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
