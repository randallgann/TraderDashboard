using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class BackTestRegistrySettings
    {
        public string[] PipStopLoss { get; set; }
        public string[] PipTakeProfit { get; set; }
        public string[] PipSlippageValue { get; set; }
        public string[] PipTrailingStopLoss { get; set; }

        public string[] MaxActiveTrades { get; set; }
    }
}
