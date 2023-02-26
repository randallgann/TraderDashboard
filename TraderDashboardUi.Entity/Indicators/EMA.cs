using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TraderDashboardUi.Entity.Indicators
{
    public class EMA
    {
        private readonly int _lookback;
        private bool _isInitialized;
        private readonly decimal _weightMultiplier;
        private decimal _previousAverage;

        public decimal Average { get; private set; }
        public decimal Slope { get; private set; }


        public EMA(int lookback)
        {
            _lookback = lookback;
            var decimalLookback = Convert.ToDecimal(lookback);
            _weightMultiplier = (decimal)2.0 / (decimalLookback + 1);
        }

        public void AddDataPoint(decimal dataPoint)
        {
            //var value = new decimal();
            //try
            //{
            //    value = decimal.Parse((dataPoint as string).Trim());
            //}
            //catch (Exception ex)
            //{
            //    var parseDecimalErr = new
            //    {
            //        Description = $"Error in EMA{_lookback} occurred while trying to parse price string into decimal.",
            //        ex.Message,
            //        ex.StackTrace
            //    };
            //}

            if(!_isInitialized)
            {
                Average = dataPoint;
                Slope = 0;
                _previousAverage = Average;
                _isInitialized = true;
                return;
            }

            double dataPointDouble = (double)dataPoint;
            int decimalPlaces = dataPointDouble.ToString().Split('.')[1].Length;
            Average = Math.Round(((dataPoint - _previousAverage) * _weightMultiplier) + _previousAverage, decimalPlaces);
            Slope = Average - _previousAverage;

            // update previous average
            _previousAverage = Average;
        }
    }
}

