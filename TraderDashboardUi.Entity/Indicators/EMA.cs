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
        private readonly double _weightMultiplier;
        private double _previousAverage;

        public double Average { get; private set; }
        public double Slope { get; private set; }


        public EMA(int lookback)
        {
            _lookback = lookback;
            _weightMultiplier = 2.0 / (lookback + 1);
        }

        public void AddDataPoint(object dataPoint)
        {
            var value = new double();
            try
            {
                value = double.Parse((dataPoint as string).Trim(), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                var parseDecimalErr = new
                {
                    Description = $"Error in EMA{_lookback} occurred while trying to parse price string into decimal.",
                    ex.Message,
                    ex.StackTrace
                };
            }

            if(!_isInitialized)
            {
                Average = value;
                Slope = 0;
                _previousAverage = Average;
                _isInitialized = true;
                return;
            }

            Average = ((value - _previousAverage) * _weightMultiplier) + _previousAverage;
            Slope = Average - _previousAverage;

            // update previous average
            _previousAverage = Average;
        }
    }
}

