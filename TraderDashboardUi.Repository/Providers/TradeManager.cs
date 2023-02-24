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
                    var trade = new Position
                    {
                        TransactionId = rand.Next(100, 1000).ToString(),
                        Time = Convert.ToDateTime(dr["Time"]),
                        Instrument = dr["Instrument"].ToString(),
                        Price = Convert.ToDouble(dr["Close"]),
                        BackTestActive = true,
                        BackTestPipStopLoss = Convert.ToDouble(dr["Close"]) + .0010,
                        BackTestPipTakeProfit = Convert.ToDouble(dr["Close"]) - .0010,
                        BackTestBuySell = "Sell"
                    };

                    tradeBook.Positions.Add(trade);
                }
                else if (dr["Signal"].ToString() == "2")
                {
                    Random rand = new Random();
                    var trade = new Position
                    {
                        TransactionId = rand.Next(100, 1000).ToString(),
                        Time = Convert.ToDateTime(dr["Time"]),
                        Instrument = dr["Instrument"].ToString(),
                        Price = Convert.ToDouble(dr["Close"]),
                        BackTestActive = true,
                        BackTestPipStopLoss = Convert.ToDouble(dr["Close"]) - .0010,
                        BackTestPipTakeProfit = Convert.ToDouble(dr["Close"]) + .0010,
                        BackTestBuySell = "Buy"
                    };

                    tradeBook.Positions.Add(trade);
                }
                
                if (tradeBook.Positions.Count > 0)
                {
                    double currentPrice = Convert.ToDouble(dr["Close"]);
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
                                    pos.BackTestClosePositionPL = pos.BackTestPipStopLoss - pos.Price;
                                }
                                else if (currentPrice >= pos.BackTestPipTakeProfit)
                                {
                                    pos.BackTestActive = false;
                                    pos.BackTestClosePositionPrice = pos.BackTestPipTakeProfit;
                                    pos.BackTestClosePositionPL = pos.BackTestPipTakeProfit - pos.Price;
                                }
                            }

                            if (pos.BackTestBuySell == "Sell")
                            {
                                if (currentPrice >= pos.BackTestPipStopLoss)
                                {
                                    pos.BackTestActive = false;
                                    pos.BackTestClosePositionPrice = pos.BackTestPipStopLoss;
                                    pos.BackTestClosePositionPL = pos.BackTestPipStopLoss - pos.Price;
                                }
                                else if (currentPrice <= pos.BackTestPipTakeProfit)
                                {
                                    pos.BackTestActive = false;
                                    pos.BackTestClosePositionPrice = pos.BackTestPipTakeProfit;
                                    pos.BackTestClosePositionPL = pos.BackTestPipTakeProfit - pos.Price;
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
