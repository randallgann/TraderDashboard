using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.Providers;
using TraderDashboardUi.Repository.Utilities;

namespace TraderDashboardUi
{
    public class Startup
    {
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Startup>();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var apiServiceRegistrySettings = new ApiServiceRegistrySettings();
            _configuration.GetSection("ApiServiceRegistrySettings").Bind(apiServiceRegistrySettings);

            var traderDashboardConfigurations = new TraderDashboardConfigurations
            {
                oandaSettings = apiServiceRegistrySettings.Oanda
            };

            services.AddSingleton(traderDashboardConfigurations);
            services.AddTransient<RestClient>();
            services.AddScoped<IOandaDataProvider, OandaDataProvider>();
            services.AddScoped<ITradeManager, TradeManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
