using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class Position
    {
        public string TransactionId { get; set; }
        public DateTime Time { get; set; }

        public string Type { get; set; }

        public string Instrument { get; set; }

        public int Units { get; set; }

        public double Price { get; set; }

        public string BidPrice { get; set; }
        public string AskPrice { get; set; }

        public double BackTestPipStopLoss { get; set; }
        public double BackTestPipTakeProfit { get; set; }
        public string BackTestBuySell { get; set; }

        public double BackTestClosePositionPrice { get; set; }
        public double BackTestClosePositionPL { get; set; }

        public bool BackTestActive { get; set; } = false;

    }
}
