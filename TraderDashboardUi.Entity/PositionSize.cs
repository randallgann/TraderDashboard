using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class PositionSize
    {
        public double RiskLimit { get; set; }
        public double AccountSize { get; set; }
        public double PipRisk { get; set; }
        public double PipValue { get; set; }
        public double Size { get; set; }

        // pipRisk * pipValue * lots traded = amount of risk
    }
}
