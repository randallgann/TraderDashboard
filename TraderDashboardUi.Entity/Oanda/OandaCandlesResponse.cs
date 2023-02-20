using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity.Oanda
{
    public class OandaCandlesResponse
    {

        public class OandaCandles
        {
            public string instrument { get; set; }
            public string granularity { get; set; }
            public Candle[] candles { get; set; }
        }

        public class Candle
        {
            public bool complete { get; set; }
            public int volume { get; set; }
            public string time { get; set; }
            public Mid mid { get; set; }
        }

        public class Mid
        {
            public string o { get; set; }
            public string h { get; set; }
            public string l { get; set; }
            public string c { get; set; }
        }

    }
}
