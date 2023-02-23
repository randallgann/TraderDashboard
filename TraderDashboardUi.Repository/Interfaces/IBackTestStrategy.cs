using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TraderDashboardUi.Entity;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface IBackTestStrategy<T>
    {
        bool IsInitialized { get; set;  }
        List<Candle> Candles { get; set; }

        T Strategy { get; set; }

        bool ExecuteTrade();

        DataTable CreateDataTable();
    }
}
