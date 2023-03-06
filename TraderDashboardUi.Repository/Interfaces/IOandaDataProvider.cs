using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TraderDashboardUi.Entity.Oanda;
using static TraderDashboardUi.Entity.Oanda.OandaAccountResponse;
using static TraderDashboardUi.Entity.Oanda.OandaCandlesResponse;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface IOandaDataProvider
    {
        Task<OandaAccount> GetOandaAccount(string accountType);
        Task<OandaCandles> GetOandaCandles(string instrument, DateTime backTestStartDate, DateTime backTestEndDate);
    }
}
