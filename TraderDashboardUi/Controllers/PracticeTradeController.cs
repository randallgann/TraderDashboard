using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Mvc;
using System;
using System.Diagnostics;
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
        private PracticeTradeViewModel _practiceTradeViewModel;
        private CancellationTokenSource _cancellationTokenSource;

        public PracticeTradeController(ILogger<PracticeTradeController> logger, IOandaDataProvider provider, TraderDashboardConfigurations configurations, ITradeManager tradeManager, PracticeTradeViewModel practiceTradeViewModel)
        {
            _logger = logger;
            _provider = provider;
            _traderDashboardConfigurations = configurations;
            _tradeManager = tradeManager;
            _practiceTradeViewModel = practiceTradeViewModel;
            _cancellationTokenSource = new CancellationTokenSource();
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
                _practiceTradeViewModel.Instrument = model.Instrument;
                _practiceTradeViewModel.Strategy = model.Strategy;
                return await StartPracticeTrading(_practiceTradeViewModel);
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
            //var eTime = _practiceTradeViewModel.elapsedTime;
            return Json(_practiceTradeViewModel.elapsedTime);
        }

        private async Task<IActionResult> StartPracticeTrading(PracticeTradeViewModel model)
        {

            model.elapsedTime = 0;
            model.isRunning = true;

            var cancellationToken = _cancellationTokenSource.Token;

            // start the loop on a separate thread
            Task.Run(() => UpdateElapsedTime(model, cancellationToken), cancellationToken);

            return await Task.FromResult(PartialView("_PracticeTradeRunning", model));
        }

        [AjaxOnly]
        [HttpPost]
        public ActionResult StopRunningPracticeTrade()
        {
            _cancellationTokenSource.Cancel();
            Debug.Write("Thread has been cancelled");
            _practiceTradeViewModel.elapsedTime = 0;
            _practiceTradeViewModel.isRunning = false;
            return Content("Stopped running practice tests");

        }


        private void UpdateElapsedTime(PracticeTradeViewModel practiceTradeViewModel, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                practiceTradeViewModel.elapsedTime++;
                Debug.WriteLine($"Elapsed time: {practiceTradeViewModel.elapsedTime}");
                Thread.Sleep(1000);
            };
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


//new Thread(() =>
//{
//    while (model.isRunning)
//    {
//        Debug.WriteLine(model.elapsedTime);
//        model.elapsedTime++;
//        Thread.Sleep(1000);
//    }
//}).Start();
