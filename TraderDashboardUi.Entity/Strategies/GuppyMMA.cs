using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TraderDashboardUi.Entity.Indicators;
using TraderDashboardUi.Entity.Interfaces;

namespace TraderDashboardUi.Entity.Strategies
{
    public class GuppyMMA : IStrategy
    {
        private string DirectionofTrend { get; set; } = null;
        private int DownTrendCount { get; set; } = 0;
        private int UpTrendCount { get; set; } = 0;
        private bool HasCrossOverOccurred { get; set; } = false;

        private decimal _3_5Delta { get; set; }
        private decimal _5_8Delta { get; set; }

        private decimal _8_10Delta { get; set; }
        private decimal _10_12Delta { get; set; }
        private decimal _12_15Delta { get; set; }
        private decimal _15_30Delta { get; set; }
        private decimal _30_35Delta { get; set; }
        private decimal _35_40Delta { get; set; }
        private decimal _40_45Delta { get; set; }
        private decimal _45_50Delta { get; set; }
        private decimal _50_60Delta { get; set; }
        private decimal _3_60Delta { get; set; }

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

        public void UpdateIndicators(DataRow dataRow, int decimalPlaces)
        {
            // remove the lists
            PropertyInfo[] emaProperties = typeof(GuppyMMA).GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                                            .Where(prop => prop.PropertyType == typeof(EMA))
                                                            .ToArray();
            try
            {
                // calculate the EMAs
                foreach (PropertyInfo emaProperty in emaProperties)
                {
                    var s = emaProperty.Name;
                    EMA ema = (EMA)emaProperty.GetValue(this);
                    ema.AddDataPoint(Convert.ToDecimal(dataRow["Close"]), decimalPlaces);
                    dataRow[emaProperty.Name] = ema.Average;
                }

                // calculate the deltas
                dataRow["_3_5Delta"] = CalculateDelta(_3EMA.Average, _5EMA.Average);
                dataRow["_5_8Delta"] = CalculateDelta(_5EMA.Average, _8EMA.Average);
                dataRow["_8_10Delta"] = CalculateDelta(_8EMA.Average, _10EMA.Average);
                dataRow["_10_12Delta"] = CalculateDelta(_10EMA.Average, _12EMA.Average);
                dataRow["_12_15Delta"] = CalculateDelta(_12EMA.Average, _15EMA.Average);
                dataRow["_15_30Delta"] = CalculateDelta(_15EMA.Average, _30EMA.Average);
                dataRow["_30_35Delta"] = CalculateDelta(_30EMA.Average, _35EMA.Average);
                dataRow["_35_40Delta"] = CalculateDelta(_35EMA.Average, _40EMA.Average);
                dataRow["_40_45Delta"] = CalculateDelta(_40EMA.Average, _45EMA.Average);
                dataRow["_45_50Delta"] = CalculateDelta(_45EMA.Average, _50EMA.Average);
                dataRow["_50_60Delta"] = CalculateDelta(_50EMA.Average, _60EMA.Average);
                dataRow["_3_60Delta"] = CalculateDelta(_3EMA.Average, _60EMA.Average);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            var signalResult = TestForSignal(dataRow);
            dataRow["Signal"] = signalResult;

            dataRow["DirectionofTrend"] = DirectionofTrend;
            dataRow["UpTrendCount"] = UpTrendCount;
            dataRow["DownTrendCount"] = DownTrendCount;

        }

        private object CalculateDelta(decimal average1, decimal average2)
        {
            return Math.Abs(average1 - average2);
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

            if (allLess)
            {
                DownTrendCount++;
                UpTrendCount = 0;
                DirectionofTrend = "DOWN";
                return 1;
            }
            if (allGreater)
            {
                UpTrendCount++;
                DownTrendCount = 0;
                DirectionofTrend = "UP";
                return 2;
            }
            DirectionofTrend = "NONE";
            DownTrendCount = 0;
            UpTrendCount = 0;
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
            dataTable.Columns.Add("_3_5Delta");
            dataTable.Columns.Add("_5_8Delta");
            dataTable.Columns.Add("_8_10Delta");
            dataTable.Columns.Add("_10_12Delta");
            dataTable.Columns.Add("_12_15Delta");
            dataTable.Columns.Add("_15_30Delta");
            dataTable.Columns.Add("_30_35Delta");
            dataTable.Columns.Add("_35_40Delta");
            dataTable.Columns.Add("_40_45Delta");
            dataTable.Columns.Add("_45_50Delta");
            dataTable.Columns.Add("_50_60Delta");
            dataTable.Columns.Add("_3_60Delta");
            dataTable.Columns.Add("DirectionofTrend");
            dataTable.Columns.Add("UpTrendCount");
            dataTable.Columns.Add("DownTrendCount");
            dataTable.Columns.Add("Signal");

            return dataTable;
        }
    }
}
