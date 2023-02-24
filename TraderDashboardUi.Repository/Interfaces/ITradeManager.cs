using System;
using System.Collections.Generic;
using System.Text;
using TraderDashboardUi.Entity;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface ITradeManager
    {
        public TradeBook tradeBook { get; set; }
    }
}
