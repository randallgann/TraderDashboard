using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class PracticeTradeSettings
    {
        public decimal PipStopLoss { get; set; }
        public decimal PipTakeProfit { get; set; }
        public decimal PipTrailingStopLoss { get; set; }
        public int MaxActiveTrades { get; set; }
    }
}
