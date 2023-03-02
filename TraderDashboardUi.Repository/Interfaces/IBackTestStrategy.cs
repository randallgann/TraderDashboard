using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TraderDashboardUi.Entity;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface IBackTestStrategy
    {
        DataTable ExecuteBackTest(DataTable dataTable, int decimalPlaces);
    }
}
