using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity
{
    public class Candle
    {
            public string Instrument { get; set; }
            public string Granularity { get; set; }
            public string Complete { get; set; }
            public string volume { get; set; }
            public string time { get; set; }
            public string open { get; set; }
            public string high { get; set; }
            public string low { get; set; }
            public string close { get; set; }
    }
}
