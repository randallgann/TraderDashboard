using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TraderDashboardUi.Entity.Interfaces;
using TraderDashboardUi.Entity.Strategies;
using TraderDashboardUi.Repository.Interfaces;

namespace TraderDashboardUi.Repository.StrategyProcessors
{
    public class PracticeTradeStrategyGuppyMMA : IPracticeTradeStrategy
    {
        private IStrategy _strategy;

        public PracticeTradeStrategyGuppyMMA()
        {
            _strategy = new GuppyMMA();
        }
        public DataTable ExecutePracticeTrade(DataTable dataTable, int decimalPlaces)
        {
            // arrange datatable
            var dt = _strategy.ArrangeDataTable(dataTable);

            foreach (DataRow dw in dt.Rows)
            {
                _strategy.UpdateIndicators(dw, decimalPlaces);
            }

            return dt;
        }
    }
}
