using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TraderDashboardUi.Entity.Interfaces
{
    public interface IStrategy
    {
        public void UpdateIndicators(DataRow datarow, int decimalPlaces);

        public int TestForSignal(DataRow datarow);

        public DataTable ArrangeDataTable(DataTable dataTable);
    }
}
