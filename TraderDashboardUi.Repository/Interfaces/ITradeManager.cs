using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TraderDashboardUi.Entity;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface ITradeManager
    {

        public decimal PipStopLoss { get; set; }
        public decimal PipTakeProfit { get; set; }

        public decimal PipSlippageValue { get; set; }
        public int Units { get; set; }

        public TradeBook BackTestExecuteTrades(DataTable dataTable, int decimalPlaces);
        public int ClosePosition();
        public int OpenPosition();

    }
}
