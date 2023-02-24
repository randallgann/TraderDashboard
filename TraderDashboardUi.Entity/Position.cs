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

        public string Units { get; set; }

        public string Price { get; set; }

        public string BidPrice { get; set; }
        public string AskPrice { get; set; }

    }
}
