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
using TraderDashboardUi;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Entity.Interfaces;
using TraderDashboardUi.Entity.Strategies;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.StrategyProcessors;
using TraderDashboardUi.Repository.Utilities;
using static TraderDashboardUi.Entity.Oanda.OandaCandlesResponse;

namespace TraderDashboardUi.Repository.Providers
{
    public class PracticeTradeThreadRunner : IDisposable
    {
        private readonly ILogger<PracticeTradeThreadRunner> _logger;
        private IOandaDataProvider _provider;
        private IStrategy _strategy;
        private Thread _thread;
        public DataTable _candlesDataTable;
        public DataTable tradingDataTable;
        public ITradeManager _tradeManager;
        //private RestClient _restClient;
        public OCandle mostRecentCandle;
        public OCandle inProgressCandle;
        public bool isRunning = false;
        public TimeSpan elapsedTime;
        public PracticeTradeSettings _practiceTradeSettings;

        public PracticeTradeThreadRunner(ILogger<PracticeTradeThreadRunner> logger, TraderDashboardConfigurations traderDashboardConfigurations, PracticeTradeSettings practiceTradeSettings, ITradeManager tradeManager, IOandaDataProvider oandaDataProvider)
        {
            this.tradingDataTable = new DataTable();
            this._thread = new Thread(StartPracticeTradingThread);
            //this._thread.Start();

            _logger = logger;
            _tradeManager = tradeManager;
            _practiceTradeSettings = practiceTradeSettings;
            _provider = oandaDataProvider;
            //_provider = new OandaDataProvider((ILogger<OandaDataProvider>)_logger, _restClient, traderDashboardConfigurations, _practiceTradeSettings);
        }

        public string instrument { get; set; }
        public string strategy { get; set; }

        public string orderType { get; set; }

        public void StartThread()
        {
            this._thread.Start();
        }

        public async void StartPracticeTradingThread()
        {
            bool isFirstIteration = true;
            var candleToAddToDataTable = new OCandle();
            int decimalPlaces = 0;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            // get appropriate IBeckTestStrategy - StrategyProcessors
            var strategyProcessor = GetStrategyProcessor(strategy);
            if (strategyProcessor == null)
            {
                throw new Exception("strategy process is null");
            }
            _strategy = strategyProcessor;

            while (isRunning)
            {
                if (isFirstIteration)
                {
                    var ocandles = await _provider.GetOandaLatestCandles(instrument);
                    // get the number of decimal places in the close value of the pair
                    decimalPlaces = ocandles.candles[0].mid.c.ToString().Split(".")[1].Length;
                    var last60MinCandles = await GetPrevious60MinutesofCandles(ocandles);
                    // create datatable of candles
                    _candlesDataTable = Utilites.ConvertOandaCandlesToDataTable(last60MinCandles);
                    stopwatch.Start();
                    tradingDataTable = _strategy.ArrangeDataTable(_candlesDataTable);
                    foreach (DataRow dr in tradingDataTable.Rows)
                    {
                        _strategy.UpdateIndicators(dr, decimalPlaces);
                    }

                    foreach(DataRow dr in tradingDataTable.Rows)
                    {
                        _tradeManager.PracticeTradeExecute(dr, orderType, decimalPlaces);
                    }
                    isFirstIteration = false;
                }
                elapsedTime = stopwatch.Elapsed;
                try
                {
                    var candles = _provider.GetOandaLatestCandles(instrument).Result;
                    mostRecentCandle = GetMostRecentCandle(candles, true);
                    inProgressCandle = GetMostRecentCandle(candles);
                    if (mostRecentCandle.complete == true && mostRecentCandle.time != candleToAddToDataTable.time)
                    {
                        candleToAddToDataTable = mostRecentCandle;
                        AddCandleToDataTable(tradingDataTable, candleToAddToDataTable, candles.instrument, candles.granularity);
                        Debug.WriteLine($"Candle Added To DataTable - Time: {mostRecentCandle.time} Price: {mostRecentCandle.mid.c} Rows: {_candlesDataTable.Rows.Count}");
                        _strategy.UpdateIndicators(tradingDataTable.Rows[tradingDataTable.Rows.Count - 1], decimalPlaces);
                        _tradeManager.PracticeTradeExecute(tradingDataTable.Rows[tradingDataTable.Rows.Count - 1], orderType, decimalPlaces);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception occured when trying to get lastest candles - {ex}");
                }
                Thread.Sleep(10000);

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

        private IStrategy GetStrategyProcessor(string strategy)
        {
            switch (strategy)
            {
                case "GUPPYMMA":
                    return new GuppyMMA();
                default:
                    return null;
            }
        }

        public void Dispose()
        {
            this.isRunning = false;
            this._thread.Join();
            this._thread = null;
        }
    }
}
