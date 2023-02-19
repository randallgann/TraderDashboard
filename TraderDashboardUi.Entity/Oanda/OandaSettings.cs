using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity.Oanda
{
    public class OandaSettings
    {
        public string BaseUrl { get; set; }
        public string Id { get; set; }
        public Dictionary<string, string> RequestHeaders { get; set; }
        public Dictionary<string, string> Endpoints { get; set; }
    }
}
