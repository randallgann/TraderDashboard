using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class Position
    {
        public string TransactionId { get; set; }
        public string Time { get; set; }

        public string Type { get; set; }

        public string Instrument { get; set; }

        public int Units { get; set; }

        public decimal Price { get; set; }

        public string BidPrice { get; set; }
        public string AskPrice { get; set; }

        public decimal BackTestPipStopLoss { get; set; }
        public decimal BackTestPipTakeProfit { get; set; }
        public string BackTestBuySell { get; set; }

        public bool BackTestWinLoss { get; set; }

        public decimal BackTestClosePositionPrice { get; set; }
        public decimal BackTestClosePositionPL { get; set; }

        public bool BackTestActive { get; set; } = false;

        public bool ActiveTrade { get; set; } = false;

        public decimal ActiveTradePipStopLoss { get; set; }
        public decimal ActiveTradePipTakeProfit { get; set; }

        public string ActiveTradeBuySell { get; set; }

        public string ActiveTradeClosePositionPrice { get; set; }
        public string ActiveTradeWinLoss { get; set; }

        public decimal ActiveTradeRealizedPL { get; set; } = 0.0m;

    }
}
