using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TraderDashboardUi;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Entity.Oanda;
using TraderDashboardUi.Repository.Interfaces;

namespace TraderDashboardUi.Repository.Providers
{
    public class TradeManager : ITradeManager
    {
        private readonly ILogger<TradeManager> _logger;
        private BackTestSettings _backTestSettings;
        private PracticeTradeSettings _practiceTradeSettings;
        private readonly IOandaDataProvider _oandaDataProvider;
        private bool _hasDirectionOfTrendChanged = false;
        private bool _isFirstCall = true;
        private object _previousDirectionOfTrend = null;
        public TradeBook tradeBook { get; set; }
        public decimal PipStopLoss { get; set; } = 0;
        public decimal PipTakeProfit { get; set; } = 0;

        public decimal PipSlippageValue { get; set; }
        public int Units { get; set; }

        public int MaxActiveTrades { get; set; }

        public int CandleCounter { get; set; } = 0;

        public TradeManager()
        {
            this.tradeBook = new TradeBook();
        }

        public TradeManager(ILogger<TradeManager> logger, BackTestSettings backTestSettings, PracticeTradeSettings practiceTradeSettings, TradeBook tradeBook, IOandaDataProvider oandaDataProvider)
        {
            this.tradeBook = tradeBook;
            _logger = logger;
            _backTestSettings = backTestSettings;
            _practiceTradeSettings = practiceTradeSettings;
            _oandaDataProvider = oandaDataProvider;
        }

        public int ClosePosition()
        {
            throw new NotImplementedException();
        }

        public TradeBook BackTestExecuteTrades(DataTable dt, int decimalPlaces)
        {
            if (tradeBook.Positions.Count != 0)
            {
                tradeBook.Positions.Clear();
            }

            var backTestSettingsList = GetSettingsCombinations();

            // convert pipcounts to pipvalues

            foreach (var backTestSettings in backTestSettingsList)
            {
                backTestSettings["PipStopLoss"] = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(backTestSettings["PipStopLoss"], decimalPlaces);
                backTestSettings["PipTakeProfit"] = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(backTestSettings["PipTakeProfit"], decimalPlaces);
                backTestSettings["PipSlippageValue"] = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(backTestSettings["PipSlippageValue"], decimalPlaces);
                backTestSettings["PipTrailingStopLoss"] = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(backTestSettings["PipTrailingStopLoss"], decimalPlaces);

                // initialize counter to start trading only after 60 candles have been calculated in order to get accurate values for 60EMA
                foreach (DataRow dr in dt.Rows)
                {
                    if (_previousDirectionOfTrend == null)
                    {
                        _previousDirectionOfTrend = dr["DirectionofTrend"];
                    }
                    else
                    {
                        object currentDirectionofTrend = dr["DirectionofTrend"];
                        if (!currentDirectionofTrend.Equals(_previousDirectionOfTrend))
                        {
                            _hasDirectionOfTrendChanged = true;
                            _previousDirectionOfTrend = currentDirectionofTrend;
                        }
                    }
                    if (CandleCounter > 60 && _hasDirectionOfTrendChanged)
                    {
                        CandleCounter++;
                        if (dr["Signal"].ToString() == "1" && tradeBook.Positions.Count(prop => prop.BackTestActive) < (int)backTestSettings["MaxActiveTrades"])
                        {
                            Random rand = new Random();
                            var tradeId = rand.Next(100, 1000).ToString();

                            var logInfo = new
                            {
                                Description = "Sell Order Created",
                                tradeId = tradeId,
                            };

                            _logger.LogInformation(JsonConvert.SerializeObject(logInfo));

                            var trade = new Position
                            {
                                TransactionId = tradeId,
                                Time = (dr["Time"]).ToString(),
                                Instrument = dr["Instrument"].ToString(),
                                Price = Convert.ToDecimal(dr["Close"]),
                                BackTestActive = true,
                                BackTestPipStopLoss = Math.Round(Convert.ToDecimal(dr["Close"]) + backTestSettings["PipStopLoss"], decimalPlaces),
                                BackTestPipTakeProfit = Math.Round(Convert.ToDecimal(dr["Close"]) - backTestSettings["PipTakeProfit"], decimalPlaces),
                                BackTestBuySell = "Sell"
                            };

                            tradeBook.Positions.Add(trade);
                        }
                        else if (dr["Signal"].ToString() == "2" && tradeBook.Positions.Count(prop => prop.BackTestActive) < (int)backTestSettings["MaxActiveTrades"])
                        {
                            Random rand = new Random();
                            var tradeId = rand.Next(100, 1000).ToString();

                            var logInfo = new
                            {
                                Description = "Buy Order Created",
                                tradeId = tradeId,
                            };

                            _logger.LogInformation(JsonConvert.SerializeObject(logInfo));

                            var trade = new Position
                            {
                                TransactionId = tradeId,
                                Time = (dr["Time"]).ToString(),
                                Instrument = dr["Instrument"].ToString(),
                                Price = Convert.ToDecimal(dr["Close"]),
                                BackTestActive = true,
                                BackTestPipStopLoss = Math.Round(Convert.ToDecimal(dr["Close"]) - backTestSettings["PipStopLoss"], decimalPlaces),
                                BackTestPipTakeProfit = Math.Round(Convert.ToDecimal(dr["Close"]) + backTestSettings["PipTakeProfit"], decimalPlaces),
                                BackTestBuySell = "Buy"
                            };

                            tradeBook.Positions.Add(trade);
                        }

                        if (tradeBook.Positions.Count > 0)
                        {
                            decimal currentPrice = Convert.ToDecimal(dr["Close"]);
                            decimal highPrice = Convert.ToDecimal(dr["High"]);
                            decimal lowPrice = Convert.ToDecimal(dr["Low"]);
                            foreach (var pos in tradeBook.Positions)
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
                    else
                    {
                        CandleCounter++;
                    }
                }
            }

            
            return tradeBook;
        }

        public void PracticeTradeExecute(DataRow dataRow, string orderType, int decimalPlaces)
        {
            if ( _isFirstCall )
            {
                tradeBook.Positions.Clear();
                _isFirstCall = false;
            }

            // convert pipcounts to pipvalues
            //
            PipStopLoss = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(_practiceTradeSettings.PipStopLoss, decimalPlaces);
            PipTakeProfit = GetPipValuesBasedOnDecimalPlacesInCurrencyPair(_practiceTradeSettings.PipTakeProfit, decimalPlaces);

            if (_previousDirectionOfTrend == null)
            {
                _previousDirectionOfTrend = dataRow["DirectionofTrend"];
            }
            else
            {
                object currentDirectionofTrend = dataRow["DirectionofTrend"];
                if (!currentDirectionofTrend.Equals(_previousDirectionOfTrend))
                {
                    _hasDirectionOfTrendChanged = true;
                    _previousDirectionOfTrend = currentDirectionofTrend;
                }
            }

            if (CandleCounter > 61)
            {
                CandleCounter++;
                // if the candle is a sell signal and the active positions count is less than maxActiveTrades
                if (dataRow["Signal"].ToString() == "1" && tradeBook.Positions.Count(prop => prop.ActiveTrade) < _practiceTradeSettings.MaxActiveTrades)
                {
                    OrderResponse response = new OrderResponse();
                    // open a sell position
                    if (orderType == "LIMIT")
                    {
                        response = _oandaDataProvider.SendSellLimitOrder(dataRow["Instrument"].ToString(), dataRow["Close"].ToString(), dataRow["Time"].ToString()).Result;
                    }
                    else
                    {
                        response = _oandaDataProvider.SendSellMarketOrder(dataRow["Instrument"].ToString()).Result;
                    }

                    // handle this case better by returning something to the view
                    if (response == null)
                    {
                        _logger.LogError("OandaDataProvider was not able to execute trade successfully");
                        return;
                    }

                    if (response.orderCancelTransaction == null && response.orderFillTransaction != null)
                    {
                        var logInfo = new
                        {
                            Description = "Buy Order Created",
                            TradeId = response.orderFillTransaction.tradeOpened.tradeID,
                            Instrument = response.orderFillTransaction.instrument,
                        };

                        _logger.LogInformation(JsonConvert.SerializeObject(logInfo));

                        var trade = new Position
                        {
                            TransactionId = response.orderFillTransaction.tradeOpened.tradeID,
                            Time = response.orderFillTransaction.time,
                            Instrument = response.orderFillTransaction.instrument,
                            Price = Convert.ToDecimal(response.orderFillTransaction.tradeOpened.price),
                            Units = Convert.ToInt32(response.orderFillTransaction.tradeOpened.units),
                            ActiveTrade = true,
                            ActiveTradePipStopLoss = Convert.ToDecimal(response.orderFillTransaction.tradeOpened.price) + PipStopLoss,
                            ActiveTradePipTakeProfit = Convert.ToDecimal(response.orderFillTransaction.tradeOpened.price) - PipTakeProfit,
                            ActiveTradeBuySell = "Sell"
                        };

                        tradeBook.Positions.Add(trade);
                    }
                }
                else if (dataRow["Signal"].ToString() == "2" && tradeBook.Positions.Count(prop => prop.ActiveTrade) < _practiceTradeSettings.MaxActiveTrades)
                {
                    OrderResponse response = new OrderResponse();
                    // open a buy position
                    if (orderType == "LIMIT")
                    {
                        response = _oandaDataProvider.SendBuyLimitOrder(dataRow["Instrument"].ToString(), dataRow["Close"].ToString(), dataRow["Time"].ToString()).Result;
                    }
                    else
                    {
                        response = _oandaDataProvider.SendSellMarketOrder(dataRow["Instrument"].ToString()).Result;
                    }

                    // handle this case better by returning something to the view
                    if (response == null)
                    {
                        _logger.LogError("OandaDataProvider was not able to execute trade successfully");
                        return;
                    }

                    if (response.orderCancelTransaction == null && response.orderFillTransaction != null)
                    {
                        var logInfo = new
                        {
                            Description = "Buy Order Created",
                            TradeId = response.orderFillTransaction.tradeOpened.tradeID,
                            Instrument = response.orderFillTransaction.instrument,
                        };

                        _logger.LogInformation(JsonConvert.SerializeObject(logInfo));

                        var trade = new Position
                        {
                            TransactionId = response.orderFillTransaction.tradeOpened.tradeID,
                            Time = response.orderFillTransaction.time,
                            Instrument = response.orderFillTransaction.instrument,
                            Price = Convert.ToDecimal(response.orderFillTransaction.tradeOpened.price),
                            Units = Convert.ToInt32(response.orderFillTransaction.tradeOpened.units),
                            ActiveTrade = true,
                            ActiveTradePipStopLoss = Convert.ToDecimal(response.orderFillTransaction.tradeOpened.price) - PipStopLoss,
                            ActiveTradePipTakeProfit = Convert.ToDecimal(response.orderFillTransaction.tradeOpened.price) + PipTakeProfit,
                            ActiveTradeBuySell = "Buy"
                        };

                        tradeBook.Positions.Add(trade);
                    }
                }
                if (tradeBook.Positions.Count > 0)
                {
                    decimal currentPrice = Convert.ToDecimal(dataRow["Close"]);
                    decimal highPrice = Convert.ToDecimal(dataRow["High"]);
                    decimal lowPrice = Convert.ToDecimal(dataRow["Low"]);
                    foreach (var pos in tradeBook.Positions)
                    {
                        if (pos.ActiveTrade)
                        {
                            if (pos.ActiveTradeBuySell == "Buy")
                            {
                                if (currentPrice <= pos.ActiveTradePipStopLoss)
                                {
                                    var response = _oandaDataProvider.CloseOrderById(pos.TransactionId).Result;
                                    _logger.LogInformation("Trade Manager - Close Order Executed to tradeFile.");

                                    if (response.orderFillTransaction.tradesClosed != null)
                                    {
                                        pos.ActiveTrade = false;
                                        pos.ActiveTradeClosePositionPrice = response.orderFillTransaction.tradesClosed.FirstOrDefault(t => t.tradeID == pos.TransactionId).price;
                                        pos.ActiveTradeRealizedPL = Math.Round(Convert.ToDecimal(response.orderFillTransaction.tradesClosed.FirstOrDefault(t => t.tradeID == pos.TransactionId).realizedPL), decimalPlaces);
                                        pos.BackTestWinLoss = false;
                                    }
                                }
                                else if (currentPrice >= pos.ActiveTradePipTakeProfit)
                                {
                                    var response = _oandaDataProvider.CloseOrderById(pos.TransactionId).Result;
                                    _logger.LogInformation("Trade Manager - Close Order Executed to tradeFile.");

                                    if (response.orderFillTransaction.tradesClosed != null)
                                    {
                                        pos.ActiveTrade = false;
                                        pos.ActiveTradeClosePositionPrice = response.orderFillTransaction.tradesClosed.FirstOrDefault(t => t.tradeID == pos.TransactionId).price;
                                        pos.ActiveTradeRealizedPL = Math.Round(Convert.ToDecimal(response.orderFillTransaction.tradesClosed.FirstOrDefault(t => t.tradeID == pos.TransactionId).realizedPL), decimalPlaces);
                                        pos.BackTestWinLoss = true;
                                    }
                                }
                            }

                            if (pos.ActiveTradeBuySell == "Sell")
                            {
                                if (currentPrice >= pos.ActiveTradePipStopLoss)
                                {
                                    var response = _oandaDataProvider.CloseOrderById(pos.TransactionId).Result;
                                    _logger.LogInformation("Trade Manager - Close Order Executed to tradeFile.");

                                    if (response.orderFillTransaction.tradesClosed != null)
                                    {
                                        pos.ActiveTrade = false;
                                        pos.ActiveTradeClosePositionPrice = response.orderFillTransaction.tradesClosed.FirstOrDefault(t => t.tradeID == pos.TransactionId).price;
                                        pos.ActiveTradeRealizedPL = Math.Round(Convert.ToDecimal(response.orderFillTransaction.tradesClosed.FirstOrDefault(t => t.tradeID == pos.TransactionId).realizedPL), decimalPlaces);
                                        pos.BackTestWinLoss = false;
                                    }
                                }
                                else if (currentPrice <= pos.ActiveTradePipTakeProfit)
                                {
                                    var response = _oandaDataProvider.CloseOrderById(pos.TransactionId).Result;
                                    _logger.LogInformation("Trade Manager - Close Order Executed to tradeFile.");

                                    if (response.orderFillTransaction.tradesClosed != null)
                                    {
                                        pos.ActiveTrade = false;
                                        pos.ActiveTradeClosePositionPrice = response.orderFillTransaction.tradesClosed.FirstOrDefault(t => t.tradeID == pos.TransactionId).price;
                                        pos.ActiveTradeRealizedPL = Math.Round(Convert.ToDecimal(response.orderFillTransaction.tradesClosed.FirstOrDefault(t => t.tradeID == pos.TransactionId).realizedPL), decimalPlaces);
                                        pos.BackTestWinLoss = true;
                                    };
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                CandleCounter++;
            }
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
