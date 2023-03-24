using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using TraderDashboardUi.Entity.Oanda.Order;

namespace TraderDashboardUi.Entity.Interfaces
{
    public interface IOrder
    {
        [JsonProperty("id")]
        string Id { get; set; }
        [JsonProperty("time")]
        DateTime CreateTime { get; set; }
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        OrderType OrderType { get; set; }
        [JsonProperty("timeInForce")]
        [JsonConverter(typeof(StringEnumConverter))]
        TimeInForce TimeInForce { get; set; }

    }
}
