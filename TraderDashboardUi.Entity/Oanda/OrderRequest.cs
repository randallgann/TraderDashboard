using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity.Oanda
{
    public class Order
    {
        [JsonProperty("units")]
        public string Units { get; set; }
        [JsonProperty("instrument")]
        public string Instrument { get; set; }
        [JsonProperty("timeInForce")]
        public string TimeInForce { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("positionFill")]
        public string PositionFill { get; set; }
    }
}
