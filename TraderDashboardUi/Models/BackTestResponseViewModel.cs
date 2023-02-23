using System.Collections.Generic;
using System.Data;
using TraderDashboardUi.Entity;

namespace TraderDashboardUi.Models
{
    public class BackTestResponseViewModel
    {
        public List<Candle> Candles { get; set; } = new List<Candle>();
    }
}
