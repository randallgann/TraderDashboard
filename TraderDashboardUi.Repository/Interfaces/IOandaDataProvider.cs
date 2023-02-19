using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TraderDashboardUi.Entity.Oanda;
using static TraderDashboardUi.Entity.Oanda.OandaAccountResponse;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface IOandaDataProvider
    {
        Task<OandaAccount> GetOandaAccount();
    }
}
