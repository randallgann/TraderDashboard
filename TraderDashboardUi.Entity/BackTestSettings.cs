﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class BackTestSettings
    {
        public decimal[] PipStopLoss { get; set; }
        public decimal[] PipTakeProfit { get; set; }
        public decimal[] PipSlippageValue { get; set; }
        public decimal[] PipTrailingStopLoss { get; set; }
        public decimal[] MaxActiveTrades { get; set; }

    }
}
