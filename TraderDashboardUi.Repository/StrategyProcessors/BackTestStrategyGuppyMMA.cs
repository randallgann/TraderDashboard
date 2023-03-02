using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        public BackTestStrategyGuppyMMA()
        { }

        public BackTestStrategyGuppyMMA(bool isInitialized, List<Candle> candles, GuppyMMA strategy)
        {
            _isInitialized = isInitialized;
            _candles = candles;
            _strategy = strategy;
        }

        public DataTable CreateDataTable()
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteBackTest(DataTable dataTable, int decimalPlaces)
        {
            // arrange datatable
            var dt = _strategy.ArrangeDataTable(dataTable);

            //DataRow[] rows = dt.Select("Time = '" + "2023-02-01T09:06:00.000000000Z" + "'");

            // calculate indicators and populate datatable
            var counter =  0;
            foreach (DataRow dw in dt.Rows)
            {
                Debug.WriteLine(dw["Time"]);
                Debug.WriteLine(counter++);
                _strategy.UpdateAllEMA(dw, decimalPlaces);
            }

            return dt;
        }

        public bool ExecuteTrade()
        {
            throw new NotImplementedException();
        }
    }
}
