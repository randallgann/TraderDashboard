using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TraderDashboardUi.Entity.Indicators;

namespace TraderDashboardUi.Entity.Strategies
{
    public class GuppyMMA
    {
        private EMA _3EMA { get; set; }
        private EMA _5EMA { get; set; }
        private EMA _8EMA { get; set; }
        private EMA _10EMA { get; set; }
        private EMA _12EMA { get; set; }
        private EMA _15EMA { get; set; }
        private EMA _30EMA { get; set; }
        private EMA _35EMA { get; set; }
        private EMA _40EMA { get; set; }
        private EMA _45EMA { get; set; }
        private EMA _50EMA { get; set; }
        private EMA _60EMA { get; set; }

        private List<EMA> ShortTermEMA { get; set; } = new List<EMA>();
        private List<EMA> LongTermEMA { get; set; } = new List<EMA>();

        public GuppyMMA()
        {
            ShortTermEMA.Add(_3EMA = new EMA(3));
            ShortTermEMA.Add(_5EMA = new EMA(5));
            ShortTermEMA.Add(_8EMA = new EMA(8));
            ShortTermEMA.Add(_10EMA = new EMA(10));
            ShortTermEMA.Add(_12EMA = new EMA(12));
            ShortTermEMA.Add(_15EMA = new EMA(15));
            LongTermEMA.Add(_30EMA = new EMA(30));
            LongTermEMA.Add(_35EMA = new EMA(35));
            LongTermEMA.Add(_40EMA = new EMA(40));
            LongTermEMA.Add(_45EMA = new EMA(45));
            LongTermEMA.Add(_50EMA = new EMA(50));
            LongTermEMA.Add(_60EMA = new EMA(60));
        }

        public void UpdateAllEMA(DataRow dataRow, int decimalPlaces)
        {
            // remove the lists
            PropertyInfo[] emaProperties = typeof(GuppyMMA).GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                                            .Where(prop => prop.PropertyType == typeof(EMA))
                                                            .ToArray();
            try
            {
                foreach (PropertyInfo emaProperty in emaProperties)
                {
                    var s = emaProperty.Name;
                    EMA ema = (EMA)emaProperty.GetValue(this);
                    ema.AddDataPoint(Convert.ToDecimal(dataRow["Close"]), decimalPlaces);
                    dataRow[emaProperty.Name] = ema.Average;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            var signalResult = TestForSignal(dataRow);
            dataRow["Signal"] = signalResult;

        }

        public int TestForSignal(DataRow dataRow)
        {
            bool allLess = true;
            bool allGreater = true;

            foreach (var shortEMA in ShortTermEMA)
            {
                foreach (var longEMA in LongTermEMA)
                {
                    if (shortEMA.Average >= longEMA.Average)
                    {
                        allLess = false;
                    }
                    if (shortEMA.Average <= longEMA.Average)
                    {
                        allGreater = false;
                    }
                }
            }

            if (allLess) return 1;
            if (allGreater) return 2;
            return 0;
        }

        public DataTable ArrangeDataTable(DataTable dataTable)
        {
            dataTable.Columns.Add("_3EMA");
            dataTable.Columns.Add("_5EMA");
            dataTable.Columns.Add("_8EMA");
            dataTable.Columns.Add("_10EMA");
            dataTable.Columns.Add("_12EMA");
            dataTable.Columns.Add("_15EMA");
            dataTable.Columns.Add("_30EMA");
            dataTable.Columns.Add("_35EMA");
            dataTable.Columns.Add("_40EMA");
            dataTable.Columns.Add("_45EMA");
            dataTable.Columns.Add("_50EMA");
            dataTable.Columns.Add("_60EMA");
            dataTable.Columns.Add("Signal");

            return dataTable;
        }
    }
}
