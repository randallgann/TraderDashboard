using System.Collections.Generic;
using System.Data;

namespace TraderDashboardUi.Models
{
    public class BackTestResponseViewModel
    {
        public List<sample> samples { get; set; } = new List<sample> { new sample() };

        public class sample
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
}
