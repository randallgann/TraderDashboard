using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TraderDashboardUi.Entity;
using TraderDashboardUi.Repository.Interfaces;

namespace TraderDashboardUi.Repository.Providers
{
    public class TradeManager : ITradeManager
    {
        public TradeBook tradeBook { get; set; }
        public int PipStopLoss { get; set; }
        public int PipTakeProfit { get; set; }
        public int Units { get; set; }

        public TradeManager()
        {
            tradeBook = new TradeBook();
        }

        public int ClosePosition()
        {
            throw new NotImplementedException();
        }

        public TradeBook BackTestExecuteTrades(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Signal"].ToString() == "1")
                {
                    Random rand = new Random();
                    int decimalPlaces = dr["Close"].ToString().Split('.')[1].Length;
                    var trade = new Position
                    {
                        TransactionId = rand.Next(100, 1000).ToString(),
                        Time = Convert.ToDateTime(dr["Time"]),
                        Instrument = dr["Instrument"].ToString(),
                        Price = Convert.ToDecimal(dr["Close"]),
                        BackTestActive = true,
                        BackTestPipStopLoss = Math.Round(Convert.ToDecimal(dr["Close"]) + (decimal).0010, decimalPlaces),
                        BackTestPipTakeProfit = Math.Round(Convert.ToDecimal(dr["Close"]) - (decimal).0010, decimalPlaces),
                        BackTestBuySell = "Sell"
                    };

                    tradeBook.Positions.Add(trade);
                }
                else if (dr["Signal"].ToString() == "2")
                {
                    Random rand = new Random();
                    int decimalPlaces = dr["Close"].ToString().Split('.')[1].Length;
                    var trade = new Position
                    {
                        TransactionId = rand.Next(100, 1000).ToString(),
                        Time = Convert.ToDateTime(dr["Time"]),
                        Instrument = dr["Instrument"].ToString(),
                        Price = Convert.ToDecimal(dr["Close"]),
                        BackTestActive = true,
                        BackTestPipStopLoss = Math.Round(Convert.ToDecimal(dr["Close"]) - (decimal).0010, decimalPlaces),
                        BackTestPipTakeProfit = Math.Round(Convert.ToDecimal(dr["Close"]) + (decimal).0010, decimalPlaces),
                        BackTestBuySell = "Buy"
                    };

                    tradeBook.Positions.Add(trade);
                }
                
                if (tradeBook.Positions.Count > 0)
                {
                    int decimalPlaces = dr["Close"].ToString().Split('.')[1].Length;
                    decimal currentPrice = Convert.ToDecimal(dr["Close"]);
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
                                }
                                else if (currentPrice >= pos.BackTestPipTakeProfit)
                                {
                                    pos.BackTestActive = false;
                                    pos.BackTestClosePositionPrice = pos.BackTestPipTakeProfit;
                                    pos.BackTestClosePositionPL = Math.Round(pos.BackTestPipTakeProfit - pos.Price, decimalPlaces);
                                }
                            }

                            if (pos.BackTestBuySell == "Sell")
                            {
                                if (currentPrice >= pos.BackTestPipStopLoss)
                                {
                                    pos.BackTestActive = false;
                                    pos.BackTestClosePositionPrice = pos.BackTestPipStopLoss;
                                    pos.BackTestClosePositionPL = Math.Round(pos.Price - pos.BackTestPipStopLoss, decimalPlaces);
                                }
                                else if (currentPrice <= pos.BackTestPipTakeProfit)
                                {
                                    pos.BackTestActive = false;
                                    pos.BackTestClosePositionPrice = pos.BackTestPipTakeProfit;
                                    pos.BackTestClosePositionPL = Math.Round(pos.Price - pos.BackTestPipTakeProfit, decimalPlaces);
                                }
                            }
                        }
                    }
                }
            }
            return tradeBook;
        }

        public int OpenPosition()
        {
            throw new NotImplementedException();
        }
    }
}
