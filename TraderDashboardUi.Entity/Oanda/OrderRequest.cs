using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity.Oanda
{
    public class Order
    {
        public string Units { get; set; }
        public string Instrument { get; set; }
        public string TimeInForce { get; set; }
        public string Type { get; set; }
        public string PositionFill { get; set; }
    }
}
