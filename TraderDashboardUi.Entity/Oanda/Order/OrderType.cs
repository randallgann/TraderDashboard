using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TraderDashboardUi.Entity.Oanda.Order
{
    public enum OrderType
    {
        MARKET,
        LIMIT,
        STOP,
        MARKET_IF_TOUCHED,
        TAKE_PROFIT,
        STOP_LOSS,
        TRAILING_STOP_LOSS,
        FIXED_PRICE
    }
}
