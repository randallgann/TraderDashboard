using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class PositionSize
    {
        public decimal RiskLimit { get; set; }
        public decimal AccountSize { get; set; }
        public decimal PipRisk { get; set; }
        public decimal PipValue { get; set; }
        public decimal Size { get; set; }

        // pipRisk * pipValue * lots traded = amount of risk
    }
}
