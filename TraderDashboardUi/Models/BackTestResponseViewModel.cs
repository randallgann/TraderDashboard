using System.Collections.Generic;
using System.Data;
using TraderDashboardUi.Entity;

namespace TraderDashboardUi.Models
{
    public class BackTestResponseViewModel
    {
        public List<Dictionary<string, object>> CandlesPlusBackTest { get; set; } = new List<Dictionary<string, object>>();

        public TradeBook PositionsList { get; set; }
        public BackTestResultStats Stats { get; set; }

        public List<string> ErrorMessage { get; set; } = new List<string>();
    }
}
