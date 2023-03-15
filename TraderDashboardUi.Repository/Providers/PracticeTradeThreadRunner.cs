using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.Utilities;
using static TraderDashboardUi.Entity.Oanda.OandaCandlesResponse;

namespace TraderDashboardUi.Repository.Providers
{
    public class PracticeTradeThreadRunner : IDisposable
    {
        private readonly ILogger<PracticeTradeThreadRunner> _logger;
        private IOandaDataProvider _provider;
        private Thread _thread;
        private DataTable _candlesDataTable;
        private TradeManager _tradeManager;
        private RestClient _restClient;
        public OCandle mostRecentCandle;
        public OCandle inProgressCandle;
        public bool isRunning = false;
        public TimeSpan elapsedTime;
        public string instrument;
        public string strategy;

        public PracticeTradeThreadRunner(string instrument, string strategy, TraderDashboardConfigurations traderDashboardConfigurations)
        {
            this._thread = new Thread(StartPracticeTradingThread);
            this._thread.Start();
            this.instrument = instrument;
            this.strategy = strategy;
            _provider = new OandaDataProvider((ILogger<OandaDataProvider>)_logger, _restClient, traderDashboardConfigurations);
            _tradeManager = new TradeManager();
        }

        public async void StartPracticeTradingThread()
        {
            bool isFirstIteration = true;
            var candleToAddToDataTable = new OCandle();
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            while (isRunning)
            {
                if (isFirstIteration)
                {
                    // get the previous 60 candles
                    Debug.WriteLine(RuntimeHelpers.GetHashCode(_provider));
                    var ocandles = await _provider.GetOandaLatestCandles(instrument);
                    // get the number of decimal places in the close value of the pair
                    int decimalPlaces = ocandles.candles[0].mid.c.ToString().Split(".")[1].Length;
                    var last60MinCandles = await GetPrevious60MinutesofCandles(ocandles);
                    // create datatable of candles
                    _candlesDataTable = Utilites.ConvertOandaCandlesToDataTable(last60MinCandles);
                    stopwatch.Start();
                    isFirstIteration = false;
                }
                elapsedTime = stopwatch.Elapsed;
                var candles = _provider.GetOandaLatestCandles(instrument).Result;
                mostRecentCandle = GetMostRecentCandle(candles, true);
                inProgressCandle = GetMostRecentCandle(candles);
                if (mostRecentCandle.complete == true && mostRecentCandle.time != candleToAddToDataTable.time)
                {
                    candleToAddToDataTable = mostRecentCandle;
                    AddCandleToDataTable(_candlesDataTable, candleToAddToDataTable, candles.instrument, candles.granularity);
                    Debug.WriteLine($"Candle Added To DataTable - Time: {mostRecentCandle.time} Price: {mostRecentCandle.mid.c} Rows: {_candlesDataTable.Rows.Count}");
                }

                Thread.Sleep(30000);

            }
        }

        private void AddCandleToDataTable(DataTable candlesDataTable, OCandle candle, string instrument, string granularity)
        {
            DataRow newRow = candlesDataTable.NewRow();

            newRow["Instrument"] = instrument;
            newRow["Granularity"] = granularity;
            newRow["Complete"] = candle.complete;
            newRow["Volume"] = candle.volume;
            newRow["Time"] = candle.time;
            newRow["Open"] = Convert.ToDecimal(candle.mid.o);
            newRow["High"] = Convert.ToDecimal(candle.mid.h);
            newRow["Low"] = Convert.ToDecimal(candle.mid.l);
            newRow["Close"] = Convert.ToDecimal(candle.mid.c);

            candlesDataTable.Rows.Add(newRow);
        }

        private OCandle GetMostRecentCandle(OandaCandles candles, bool completed = false)
        {
            var maxTimeStamp = candles.candles.Max(c => DateTime.Parse(c.time));
            if (completed)
            {
                maxTimeStamp = maxTimeStamp.AddMinutes(-1);
            }
            var mostRecentCandle = candles.candles.Where(c => DateTime.Parse(c.time) == maxTimeStamp).FirstOrDefault();
            return mostRecentCandle;

        }

        private async Task<OandaCandles> GetPrevious60MinutesofCandles(OandaCandles candles)
        {
            var currentTime = DateTime.Now;
            var sixtyMinutesAgo = currentTime.AddMinutes(-60);
            var previousCandles = candles.candles.Where(c => DateTime.Parse(c.time) >= sixtyMinutesAgo && DateTime.Parse(c.time) < currentTime)
                                                .OrderBy(c => DateTime.Parse(c.time))
                                                .ToList();

            var ocandles = new OandaCandles
            {
                candles = previousCandles.Select(c => new OCandle
                {
                    time = c.time,
                    complete = c.complete,
                    volume = c.volume,
                    mid = new Mid
                    {
                        o = c.mid.o,
                        h = c.mid.h,
                        l = c.mid.l,
                        c = c.mid.c
                    },
                }).ToArray()
            };

            return ocandles;
        }

        public void Dispose()
        {
            this.isRunning = false;
            this._thread.Join();
            this._thread = null;
        }
    }
}
