using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface IPracticeTradeStrategy
    {
        DataTable ExecutePracticeTrade(DataTable dataTable, int decimalPlaces);
    }
}
