using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Entity.Strategies;
using TraderDashboardUi.Repository.Interfaces;

namespace TraderDashboardUi.Repository.StrategyProcessors
{
    public class BackTestStrategyGuppyMMA : IBackTestStrategy<GuppyMMA>
    {
        public bool IsInitialized { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Candle> Candles { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GuppyMMA Strategy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DataTable CreateDataTable()
        {
            throw new NotImplementedException();
        }

        public bool ExecuteTrade()
        {
            throw new NotImplementedException();
        }
    }
}
