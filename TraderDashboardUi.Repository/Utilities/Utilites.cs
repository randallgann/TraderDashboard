using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TraderDashboardUi.Entity;
using static TraderDashboardUi.Entity.Oanda.OandaCandlesResponse;

namespace TraderDashboardUi.Repository.Utilities
{
    public class Utilites
    {
        public static async Task<string> GetApiResponse(RestClient restClient, RestRequest request)
        {
            var response = await restClient.ExecuteAsync(request);
            if (response.ErrorException != null)
            {
                throw new Exception(response.Content);
            }
            return response.Content;
        }
        
        public static DataTable ConvertOandaCandlesToDataTable(OandaCandles ocandles)
        {
            // create a new DataTable
            DataTable dt = new DataTable();

            // add columns to the DataTable
            dt.Columns.Add("Instrument", typeof(string));
            dt.Columns.Add("Granularity", typeof(string));
            dt.Columns.Add("Complete", typeof(bool));
            dt.Columns.Add("Volume", typeof(int));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Open", typeof(decimal));
            dt.Columns.Add("High", typeof(decimal));
            dt.Columns.Add("Low", typeof(decimal));
            dt.Columns.Add("Close", typeof(decimal));

            // loop through the candles array and add a new row for each candle
            foreach (OCandle candle in ocandles.candles)
            {
                DataRow dr = dt.NewRow();

                // update the time to CST
                dr["Instrument"] = ocandles.instrument;
                dr["Granularity"] = ocandles.granularity;
                dr["Complete"] = candle.complete;
                dr["Volume"] = candle.volume;
                dr["Time"] = candle.time;
                dr["Open"] = Convert.ToDecimal(candle.mid.o);
                dr["High"] = Convert.ToDecimal(candle.mid.h);
                dr["Low"] = Convert.ToDecimal(candle.mid.l);
                dr["Close"] = Convert.ToDecimal(candle.mid.c);
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private static object UpdateTime(string time)
        {
            DateTime utcTime = DateTime.Parse(time);
            utcTime = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day, utcTime.Hour, utcTime.Minute, 0); // Remove seconds
            string utcTimeString = utcTime.ToString("yyyy-MM-ddTHH:mm:ss"); // Convert to string in CST format without time zone indicator
            //object formattedTimeString = new { Time = cstTimeString };
            return utcTimeString;
        }

        public static List<Dictionary<string, object>> ConvertBackTestResultsToDictionary(DataTable backTestResults, string strategy)
        {
            List<Dictionary<string, object>> backTestResultsList = new List<Dictionary<string, object>>();
            switch (strategy)
            {
                case "GUPPYMMA":
                    foreach (DataRow row in backTestResults.Rows)
                    {
                        var rowDict = new Dictionary<string, object>();
                        rowDict["Time"] = row["Time"];
                        rowDict["Instrument"] = row["Instrument"];
                        rowDict["Granulatiry"] = row["Granularity"];
                        rowDict["Volume"] = row["Volume"];
                        rowDict["Open"] = row["Open"];
                        rowDict["High"] = row["High"];
                        rowDict["Low"] = row["Low"];
                        rowDict["Close"] = row["Close"];
                        rowDict["3EMA"] = row["_3EMA"];
                        rowDict["5EMA"] = row["_5EMA"];
                        rowDict["8EMA"] = row["_8EMA"];
                        rowDict["10EMA"] = row["_10EMA"];
                        rowDict["12EMA"] = row["_12EMA"];
                        rowDict["15EMA"] = row["_15EMA"];
                        rowDict["30EMA"] = row["_30EMA"];
                        rowDict["35EMA"] = row["_35EMA"];
                        rowDict["40EMA"] = row["_40EMA"];
                        rowDict["45EMA"] = row["_45EMA"];
                        rowDict["50EMA"] = row["_50EMA"];
                        rowDict["60EMA"] = row["_60EMA"];
                        backTestResultsList.Add(rowDict);
                    }
                    return backTestResultsList;
                default:
                    return null;
            }
        }

        public static BackTestResultStats CalculateBackTestStats(TradeBook backTestTrades)
        {
            BackTestResultStats backTestResultStats = new BackTestResultStats();
            backTestResultStats.TotalPL = backTestTrades.Positions.Sum(p => p.BackTestClosePositionPL);
            int numberofWins = backTestTrades.Positions.Count(p => p.BackTestWinLoss);
            int numberofLoss = backTestTrades.Positions.Count(p => !p.BackTestWinLoss);
            int totalTradesExecuted = backTestTrades.Positions.Count();
            backTestResultStats.WinLossRatio = (decimal)numberofWins / backTestTrades.Positions.Count * 100;
            backTestResultStats.TotalLoss = numberofLoss;
            backTestResultStats.TotalWins = numberofWins;
            backTestResultStats.TotalTradesExecuted = totalTradesExecuted;
            return backTestResultStats;
        }
    }
}
