using static TraderDashboardUi.Entity.Oanda.OandaAccountResponse;

namespace TraderDashboardUi.Models
{
    public class HomeAccountsViewModel
    {
        public OandaAccount LiveAccountOanda { get; set; }

        public OandaAccount PracticeAccountOanda { get; set; }
    }
}
