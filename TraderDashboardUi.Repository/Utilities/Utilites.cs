using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static TraderDashboardUi.Entity.Oanda.OandaCandlesResponse;

namespace TraderDashboardUi.Repository.Utilities
{
    public class Utilites
    {
        public static async Task<string> GetApiResponse(RestClient restClient, RestRequest request)
        {
            var response = await restClient.ExecuteAsync(request);
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
            foreach (Candle candle in ocandles.candles)
            {
                DataRow dr = dt.NewRow();
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
    }
}
