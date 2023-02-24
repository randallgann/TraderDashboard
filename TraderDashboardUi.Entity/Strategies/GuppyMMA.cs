using System.Collections.Generic;
using System.Data;
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
            //_3EMA = new EMA(3);
            ShortTermEMA.Add(_3EMA = new EMA(3));
            //_5EMA = new EMA(5);
            ShortTermEMA.Add(_5EMA = new EMA(5));
            //_8EMA = new EMA(8);
            ShortTermEMA.Add(_8EMA = new EMA(8));
            //_10EMA = new EMA(10);
            ShortTermEMA.Add(_10EMA = new EMA(10));
            //_12EMA = new EMA(12);
            ShortTermEMA.Add(_12EMA = new EMA(12));
            //_15EMA = new EMA(15);
            ShortTermEMA.Add(_15EMA = new EMA(15));
            //_30EMA = new EMA(30);
            LongTermEMA.Add(_30EMA = new EMA(30));
            //_35EMA = new EMA(35);
            LongTermEMA.Add(_35EMA = new EMA(35));
            //_40EMA = new EMA(40);
            LongTermEMA.Add(_40EMA = new EMA(40));
            //_45EMA = new EMA(45);
            LongTermEMA.Add(_45EMA = new EMA(45));
            //_50EMA = new EMA(50);
            LongTermEMA.Add(_50EMA = new EMA(50));
            //_60EMA = new EMA(60);
            LongTermEMA.Add(_60EMA = new EMA(60));
        }

        public void UpdateAllEMA(DataRow dataRow)
        {
            // remove the lists
            foreach (EMA ema in ShortTermEMA)
            {
                ema.AddDataPoint(dataRow["Close"]);
            }
            foreach (EMA ema in LongTermEMA)
            {
                ema.AddDataPoint(dataRow["Close"]);
            }

        }

        public int TestForSignal()
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
            dataTable.Columns.Add("EMA3");
            dataTable.Columns.Add("EMA5");
            dataTable.Columns.Add("EMA8");
            dataTable.Columns.Add("EMA10");
            dataTable.Columns.Add("EMA12");
            dataTable.Columns.Add("EMA15");
            dataTable.Columns.Add("EMA30");
            dataTable.Columns.Add("EMA35");
            dataTable.Columns.Add("EMA40");
            dataTable.Columns.Add("EMA45");
            dataTable.Columns.Add("EMA50");
            dataTable.Columns.Add("EMA60");
            dataTable.Columns.Add("Signal");

            return dataTable;
        }
    }
}
