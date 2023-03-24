using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TraderDashboardUi.Entity.Interfaces;

namespace TraderDashboardUi.Entity.Oanda.Order
{
    public class LimitOrder : IOrder
    {
        public string Id { get; set; }
        public OrderType OrderType { get; set; } = OrderType.LIMIT;
        public DateTime CreateTime { get; set; }
        [JsonProperty("state")]
        public OrderState OrderState { get; set; }

        [JsonProperty("instrument")]
        public string Instrument { get; set; }
        [JsonProperty("units")]
        public string Units { get; set; }
        [JsonProperty("price")]
        public string Price { get; set; }

        public TimeInForce TimeInForce { get; set; }
        public string PositionFill { get; set; }
        [JsonProperty("gtdTime")]
        public string GtdTime { get; set; }

    }
}
