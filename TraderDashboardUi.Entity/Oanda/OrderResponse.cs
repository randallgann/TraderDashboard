using System;
using System.Collections.Generic;
using System.Text;

namespace TraderDashboardUi.Entity.Oanda
{
    public class OrderResponse
    { 
        public Ordercreatetransaction orderCreateTransaction { get; set; }

        public Ordercanceltransaction orderCancelTransaction { get; set; }
        public Orderfilltransaction orderFillTransaction { get; set; }
        public string[] relatedTransactionIDs { get; set; }
        public string lastTransactionID { get; set; }

        public class Ordercreatetransaction
        {
            public string id { get; set; }
            public string accountID { get; set; }
            public int userID { get; set; }
            public string batchID { get; set; }
            public string requestID { get; set; }
            public string time { get; set; }
            public string type { get; set; }
            public string instrument { get; set; }
            public string units { get; set; }
            public string timeInForce { get; set; }
            public string positionFill { get; set; }
            public string reason { get; set; }
        }

        public class Orderfilltransaction
        {
            public string id { get; set; }
            public string accountID { get; set; }
            public int userID { get; set; }
            public string batchID { get; set; }
            public string requestID { get; set; }
            public string time { get; set; }
            public string type { get; set; }
            public string orderID { get; set; }
            public string instrument { get; set; }
            public string units { get; set; }
            public string requestedUnits { get; set; }
            public string price { get; set; }
            public string pl { get; set; }
            public string quotePL { get; set; }
            public string financing { get; set; }
            public string baseFinancing { get; set; }
            public string commission { get; set; }
            public string accountBalance { get; set; }
            public string gainQuoteHomeConversionFactor { get; set; }
            public string lossQuoteHomeConversionFactor { get; set; }
            public string guaranteedExecutionFee { get; set; }
            public string quoteGuaranteedExecutionFee { get; set; }
            public string halfSpreadCost { get; set; }
            public string fullVWAP { get; set; }
            public string reason { get; set; }
            public Tradeopened tradeOpened { get; set; }
            public Tradesclosed[] tradesClosed { get; set; }
            public Fullprice fullPrice { get; set; }
            public Homeconversionfactors homeConversionFactors { get; set; }
        }

        public class Ordercanceltransaction
        {
            public string id { get; set; }
            public string accountID { get; set; }
            public int userID { get; set; }
            public string batchID { get; set; }
            public string requestID { get; set; }
            public string time { get; set; }
            public string type { get; set; }
            public string orderID { get; set; }
            public string reason { get; set; }
        }

        public class Tradeopened
        {
            public string price { get; set; }
            public string tradeID { get; set; }
            public string units { get; set; }
            public string guaranteedExecutionFee { get; set; }
            public string quoteGuaranteedExecutionFee { get; set; }
            public string halfSpreadCost { get; set; }
            public string initialMarginRequired { get; set; }
        }

        public class Tradesclosed
        {
            public string tradeID { get; set; }
            public string units { get;  set; }
            public string realizedPL { get; set; }
            public string financing { get; set; }
            public string baseFinancing { get; set; }
            public string price { get; set; }
            public string guaranteedExecutionFee { get; set; }
            public string quoteGuaranteedExecutionFee { get; set; }
            public string halfSpreadCost { get; set; }
        }

        public class Fullprice
        {
            public string closeoutBid { get; set; }
            public string closeoutAsk { get; set; }
            public string timestamp { get; set; }
            public Bid[] bids { get; set; }
            public Ask[] asks { get; set; }
        }

        public class Bid
        {
            public string price { get; set; }
            public string liquidity { get; set; }
        }

        public class Ask
        {
            public string price { get; set; }
            public string liquidity { get; set; }
        }

        public class Homeconversionfactors
        {
            public Gainquotehome gainQuoteHome { get; set; }
            public Lossquotehome lossQuoteHome { get; set; }
            public Gainbasehome gainBaseHome { get; set; }
            public Lossbasehome lossBaseHome { get; set; }
        }

        public class Gainquotehome
        {
            public string factor { get; set; }
        }

        public class Lossquotehome
        {
            public string factor { get; set; }
        }

        public class Gainbasehome
        {
            public string factor { get; set; }
        }

        public class Lossbasehome
        {
            public string factor { get; set; }
        }

    }
}
