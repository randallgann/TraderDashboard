using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Repository.Interfaces;

namespace TraderDashboardUi.Repository.Providers
{
    public class TradeManager : ITradeManager
    {
        private BackTestSettings _backTestSettings;
        public TradeBook _tradeBook { get; set; }
        public decimal PipStopLoss { get; set; }
        public decimal PipTakeProfit { get; set; }

        public decimal PipSlippageValue { get; set; }
        public int Units { get; set; }

        public int MaxActiveTrades { get; set; }

        

        public TradeManager(BackTestSettings backTestSettings, TradeBook tradeBook)
        {
            _tradeBook = tradeBook;
            _backTestSettings = backTestSettings;
        }

        public int ClosePosition()
        {
            throw new NotImplementedException();
        }

        public TradeBook BackTestExecuteTrades(DataTable dt, int decimalPlaces)
        {
            if (_tradeBook.Positions.Count != 0)
            {
                _tradeBook.Positions.Clear();
            }

            var backTestSettingsList = GetSettingsCombinations();

            // convert pipcounts to pipvalues

            foreach (var backTestSettings in backTestSettingsList)
            {
                backTestSettings["PipStopLoss"] = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(backTestSettings["PipStopLoss"], decimalPlaces);
                backTestSettings["PipTakeProfit"] = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(backTestSettings["PipTakeProfit"], decimalPlaces);
                backTestSettings["PipSlippageValue"] = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(backTestSettings["PipSlippageValue"], decimalPlaces);
                backTestSettings["PipTrailingStopLoss"] = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(backTestSettings["PipTrailingStopLoss"], decimalPlaces);

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Signal"].ToString() == "1" && _tradeBook.Positions.Count(prop => prop.BackTestActive) < (int)backTestSettings["MaxActiveTrades"])
                    {
                        Random rand = new Random();
                        
                        var trade = new Position
                        {
                            TransactionId = rand.Next(100, 1000).ToString(),
                            Time = Convert.ToDateTime(dr["Time"]),
                            Instrument = dr["Instrument"].ToString(),
                            Price = Convert.ToDecimal(dr["Close"]),
                            BackTestActive = true,
                            BackTestPipStopLoss = Math.Round(Convert.ToDecimal(dr["Close"]) + backTestSettings["PipStopLoss"], decimalPlaces),
                            BackTestPipTakeProfit = Math.Round(Convert.ToDecimal(dr["Close"]) - backTestSettings["PipTakeProfit"], decimalPlaces),
                            BackTestBuySell = "Sell"
                        };

                        _tradeBook.Positions.Add(trade);
                    }
                    else if (dr["Signal"].ToString() == "2" && _tradeBook.Positions.Count(prop => prop.BackTestActive) < (int)backTestSettings["MaxActiveTrades"])
                    {
                        Random rand = new Random();
                        var trade = new Position
                        {
                            TransactionId = rand.Next(100, 1000).ToString(),
                            Time = Convert.ToDateTime(dr["Time"]),
                            Instrument = dr["Instrument"].ToString(),
                            Price = Convert.ToDecimal(dr["Close"]),
                            BackTestActive = true,
                            BackTestPipStopLoss = Math.Round(Convert.ToDecimal(dr["Close"]) - backTestSettings["PipStopLoss"], decimalPlaces),
                            BackTestPipTakeProfit = Math.Round(Convert.ToDecimal(dr["Close"]) + backTestSettings["PipTakeProfit"], decimalPlaces),
                            BackTestBuySell = "Buy"
                        };

                        _tradeBook.Positions.Add(trade);
                    }

                    if (_tradeBook.Positions.Count > 0)
                    {
                        decimal currentPrice = Convert.ToDecimal(dr["Close"]);
                        foreach (var pos in _tradeBook.Positions)
                        {
                            if (pos.BackTestActive)
                            {
                                if (pos.BackTestBuySell == "Buy")
                                {
                                    if (currentPrice <= pos.BackTestPipStopLoss)
                                    {
                                        pos.BackTestActive = false;
                                        pos.BackTestClosePositionPrice = pos.BackTestPipStopLoss;
                                        pos.BackTestClosePositionPL = Math.Round(pos.BackTestPipStopLoss - pos.Price, decimalPlaces);
                                        pos.BackTestClosePositionPL = GetPLValueBasedOnDecimalPlacesInCurrencyPair(pos.BackTestClosePositionPL, decimalPlaces);
                                        pos.BackTestWinLoss = false;
                                    }
                                    else if (currentPrice >= pos.BackTestPipTakeProfit)
                                    {
                                        pos.BackTestActive = false;
                                        pos.BackTestClosePositionPrice = pos.BackTestPipTakeProfit;
                                        pos.BackTestClosePositionPL = Math.Round(pos.BackTestPipTakeProfit - pos.Price, decimalPlaces);
                                        pos.BackTestClosePositionPL = GetPLValueBasedOnDecimalPlacesInCurrencyPair(pos.BackTestClosePositionPL, decimalPlaces);
                                        pos.BackTestWinLoss = true;
                                    }
                                }

                                if (pos.BackTestBuySell == "Sell")
                                {
                                    if (currentPrice >= pos.BackTestPipStopLoss)
                                    {
                                        pos.BackTestActive = false;
                                        pos.BackTestClosePositionPrice = pos.BackTestPipStopLoss;
                                        pos.BackTestClosePositionPL = Math.Round(pos.Price - pos.BackTestPipStopLoss, decimalPlaces);
                                        pos.BackTestClosePositionPL = GetPLValueBasedOnDecimalPlacesInCurrencyPair(pos.BackTestClosePositionPL, decimalPlaces);
                                        pos.BackTestWinLoss = false;
                                    }
                                    else if (currentPrice <= pos.BackTestPipTakeProfit)
                                    {
                                        pos.BackTestActive = false;
                                        pos.BackTestClosePositionPrice = pos.BackTestPipTakeProfit;
                                        pos.BackTestClosePositionPL = Math.Round(pos.Price - pos.BackTestPipTakeProfit, decimalPlaces);
                                        pos.BackTestClosePositionPL = GetPLValueBasedOnDecimalPlacesInCurrencyPair(pos.BackTestClosePositionPL, decimalPlaces);
                                        pos.BackTestWinLoss = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            
            return _tradeBook;
        }

        public int OpenPosition()
        {
            throw new NotImplementedException();
        }

        public List<Dictionary<string, decimal>> GetSettingsCombinations()
        {
            var result = new List<Dictionary<string, decimal>>();

            for (int i = 0; i < _backTestSettings.PipStopLoss.Length; i++)
            {
                for (int j = 0; j < _backTestSettings.PipTakeProfit.Length; j++)
                {
                    for (int k = 0; k < _backTestSettings.PipSlippageValue.Length; k++)
                    {
                        for (int l = 0; l < _backTestSettings.PipTrailingStopLoss.Length; l++)
                        {
                            for (int m = 0; m < _backTestSettings.MaxActiveTrades.Length; m++)
                            {

                                var dictionary = new Dictionary<string, decimal>
                                {
                                    { "PipStopLoss", _backTestSettings.PipStopLoss[i] },
                                    { "PipTakeProfit", _backTestSettings.PipTakeProfit[j] },
                                    { "PipSlippageValue", _backTestSettings.PipSlippageValue[k] },
                                    { "PipTrailingStopLoss", _backTestSettings.PipTrailingStopLoss[l] },
                                    { "MaxActiveTrades", _backTestSettings.MaxActiveTrades[m] }

                                };

                                result.Add(dictionary);
                            }
                        }
                    }
                }

            }

            return result;
        }

        public decimal GetPipValuesBasedOnDecimalPlacesInCurrencyPair(decimal pipSettingsCount, int decimalPlaces)
        {
            decimal result = 1m / (decimal)Math.Pow(10, decimalPlaces);
            return decimal.Multiply(pipSettingsCount, result);
        }

        public int GetPLValueBasedOnDecimalPlacesInCurrencyPair(decimal tradePL, int decimalPlaces)
        {
            decimal result = (decimal)Math.Pow(10, decimalPlaces);
            decimal multipliedValue =  decimal.Multiply(tradePL, result);
            int roundedValue = (int)multipliedValue;
            return roundedValue;
        }
    }
}
