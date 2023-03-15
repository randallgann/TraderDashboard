using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Entity.Oanda;
using TraderDashboardUi.Repository.Providers;
using TraderDashboardUi.Repository.Utilities;

namespace TraderDashboardUi.Repository.Tests.Providers
{
    [TestClass]
    public class OandaDataProviderTest
    {
        private NullLogger<OandaDataProvider> _logger;
        private OandaDataProvider _provider;

        [TestInitialize]
        public void Setup()
        {
            _logger = new NullLogger<OandaDataProvider>();
            var restClient = new RestClient();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("C:\\Users\\Randall\\OneDrive\\Documents\\Dev\\TraderDashboard\\TraderDashboardUi\\appsettings.Development.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var apiServiceRegistrySettings = new ApiServiceRegistrySettings();
            configuration.GetSection("ApiServiceRegistrySettings").Bind(apiServiceRegistrySettings);

            var traderDashboardConfigurations = new TraderDashboardConfigurations
            {
                oandaSettings = apiServiceRegistrySettings.Oanda
            };

            _provider = new OandaDataProvider(_logger, restClient, traderDashboardConfigurations);
        }

        [TestMethod]
        public async Task GetOandaAccount_ReturnsOandaAccountResponse()
        {
            // Act
            var result = await _provider.GetOandaAccount("live");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OandaAccountResponse));
        }
    }
}
