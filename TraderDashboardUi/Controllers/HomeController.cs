using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TraderDashboardUi.Entity.Oanda;
using TraderDashboardUi.Models;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.Utilities;
using static TraderDashboardUi.Entity.Oanda.OandaAccountResponse;

namespace TraderDashboardUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOandaDataProvider _provider;
        private readonly TraderDashboardConfigurations _traderDashboardConfigurations;

        public HomeController(ILogger<HomeController> logger, IOandaDataProvider provider, TraderDashboardConfigurations configurations)
        {
            _logger = logger;
            _provider = provider;
            _traderDashboardConfigurations = configurations;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = GetInitialModel();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private HomeAccountsViewModel GetInitialModel()
        {
            OandaAccount liveAccountResponse = _provider.GetOandaAccount("live").Result;
            OandaAccount practiceAccountResponse = _provider.GetOandaAccount("live").Result;

            var model = new HomeAccountsViewModel();
            model.LiveAccountOanda = liveAccountResponse;
            model.PracticeAccountOanda = practiceAccountResponse;

            return new HomeAccountsViewModel
            {
                LiveAccountOanda = liveAccountResponse,
                PracticeAccountOanda = practiceAccountResponse
            };

        }
    }
}
