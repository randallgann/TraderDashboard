﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TraderDashboardUi.Entity.Oanda;
using static TraderDashboardUi.Entity.Oanda.OandaAccountResponse;
using static TraderDashboardUi.Entity.Oanda.OandaCandlesResponse;

namespace TraderDashboardUi.Repository.Interfaces
{
    public interface IOandaDataProvider
    {
        Task<OandaAccount> GetOandaAccount(string accountType);
        Task<OandaCandles> GetOandaCandles(string instrument, DateTime backTestStartDate, DateTime backTestEndDate);

        Task<OandaCandles> GetOandaLatestCandles(string instrument);

        Task<OrderResponse> SendSellOrder(string instrument);
        Task<OrderResponse> SendBuyOrder(string instrument);

        Task<OrderResponse> CloseOrderById(string orderId);
    }
}
