using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class BackTestResultStats
    {
        public int TotalWins { get; set; }
        public int TotalLoss { get; set; }
        public int TotalTradesExecuted { get; set; }

        public decimal TotalPL { get; set; }
        public decimal WinLossRatio { get; set; }
        
    }
}
