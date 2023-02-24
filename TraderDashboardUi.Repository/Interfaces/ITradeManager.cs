using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TraderDashboardUi.Entity;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface ITradeManager
    {
        public TradeBook tradeBook { get; set; }

        public int PipStopLoss { get; set; }
        public int PipTakeProfit { get; set; }
        public int Units { get; set; }

        public TradeBook BackTestExecuteTrades(DataTable dataTable);
        public int ClosePosition();
        public int OpenPosition();

    }
}
