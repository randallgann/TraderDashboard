using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Entity.Strategies;
using TraderDashboardUi.Repository.Interfaces;
using TraderDashboardUi.Repository.Providers;

namespace TraderDashboardUi.Repository.StrategyProcessors
{
    public class BackTestStrategyGuppyMMA : IBackTestStrategy
    {
        private bool _isInitialized = false;
        private List<Candle> _candles = new List<Candle>();
        private GuppyMMA _strategy = new GuppyMMA();
        private ITradeManager _tradeManager = new TradeManager();

        public BackTestStrategyGuppyMMA()
        { }

        public BackTestStrategyGuppyMMA(bool isInitialized, List<Candle> candles, GuppyMMA strategy, ITradeManager tradeManager)
        {
            _isInitialized = isInitialized;
            _candles = candles;
            _strategy = strategy;
            _tradeManager = tradeManager;
        }

        public DataTable CreateDataTable()
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteBackTest(DataTable dataTable)
        {
            // arrange datatable
            var dt = _strategy.ArrangeDataTable(dataTable);

            // calculate indicators and populate datatable
            foreach (DataRow dw in dt.Rows)
            {
                _strategy.UpdateAllEMA(dw);

                // if signal - execute trade
            }
            return dt;
        }

        public bool ExecuteTrade()
        {
            throw new NotImplementedException();
        }
    }
}
