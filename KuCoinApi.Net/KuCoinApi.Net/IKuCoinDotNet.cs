// -----------------------------------------------------------------------------
// <copyright file="IKuCoinDotNet" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/21/2019 7:49:58 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net
{
    #region Usings

    using KuCoinApi.Net.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    #endregion Usings

    public interface IKuCoinDotNet
    {
        #region Secure Endpoints
        
        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="type">Account type</param>
        /// <returns>Balance collection</returns>
        Task<List<Balance>> GetBalances(string symbol, AccountType? type);

        /// <summary>
        /// Get account balance of an account
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <returns>Balance object</returns>
        Task<Balance> GetBalance(string accountId);

        /// <summary>
        /// Create an account
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="type">Type of account</param>
        /// <returns>Id of new account</returns>
        Task<string> CreateAccount(string symbol, AccountType type);

        /// <summary>
        /// Get account history
        /// </summary>
        /// <param name="accountId">id of account</param>
        /// <param name="startAt">Unix start time</param>
        /// <param name="endAt">Unix end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged response of account history</returns>
        Task<PagedResponse<List<AccountAction>>> GetAccountHistory(string accountId, long startAt, long endAt, int page = 0, int pageSize = 0);
        
        /// <summary>
        /// Get holds on an account
        /// </summary>
        /// <param name="accountId">id of account</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged response of account holds</returns>
        Task<PagedResponse<List<AccountHold>>> GetHolds(string accountId, int page = 0, int pageSize = 0);

        /// <summary>
        /// Transfer funds between accounts
        /// </summary>
        /// <param name="clientOid">Request Id</param>
        /// <param name="fromId">Account Id Payer</param>
        /// <param name="toId">Account Id Receiver</param>
        /// <param name="amount">Amount to transfer</param>
        /// <returns>Id of funds transfer order</returns>
        Task<string> InnerTransfer(string clientOid, string fromId, string toId, decimal amount);
        
        /// <summary>
        /// Place an order
        /// </summary>
        /// <param name="body">Body of order message</param>
        /// <returns>String of order id</returns>
        Task<string> PlaceOrder(SortedDictionary<string, object> body);

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="orderId">Id of order to cancel</param>
        /// <returns>Id of order canceled</returns>
        Task<string> CancelOrder(string orderId);

        /// <summary>
        /// Cancel all open orders
        /// </summary>
        /// <returns>Collection of canceled order Ids</returns>
        Task<List<string>> CancelAllOrders();

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="status">Order status</param>
        /// <param name="side">Trade side</param>
        /// <param name="type">Order Type</param>
        /// <param name="startAt">Start Date (Unix time)</param>
        /// <param name="endAt">End Date (Unix time)</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOrders(OrderStatus? status = null, string pair = null, Side? side = null, OrderType? type = null, long startAt = 0, long endAt = 0, int page = 1, int pageSize = 50);

        /// <summary>
        /// Get an order
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns>An Order</returns>
        Task<Order> GetOrder(string orderId);

        /// <summary>
        /// Get a list of KuCoin V1 historical orders.
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Side of trade</param>
        /// <param name="startAt">Start Date (Unix time)</param>
        /// <param name="endAt">End Date (Unix time)</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<HistoricOrder>>> GetHistoricOrders(string pair = null, Side? side = null, long startAt = 0, long endAt = 0, int page = 1, int pageSize = 50);

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="type">Order type</param>
        /// <param name="startAt">Start date</param>
        /// <param name="endAt">End date</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Page collection of Fills</returns>
        Task<PagedResponse<List<Fill>>> GetFills(string orderId = null, string pair = null, Side? side = null, OrderType? type = null, long startAt = 0, long endAt = 0, int page = 0, int pageSize = 0);

        /// <summary>
        /// Create Deposit address for a currency
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>New Deposit Address</returns>
        Task<DepositAddress> CreateDepositAddress(string symbol);

        /// <summary>
        /// Get Deposit address for a currency
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Deposit Address</returns>
        Task<DepositAddress> GetDepositAddress(string symbol);

        /// <summary>
        /// Get deposit history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="startAt">Start date</param>
        /// <param name="endAt">End date</param>
        /// <param name="status">Deposit status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        Task<PagedResponse<List<Deposit>>> GetDepositHistory(string symbol = null, long startAt = 0, long endAt = 0, DepositStatus? status = null, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get withdrawal history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="startAt">Start date</param>
        /// <param name="endAt">End date</param>
        /// <param name="status">Withdrawal status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(string symbol = null, long startAt = 0, long endAt = 0, WithdrawalStatus? status = null, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get withdrawal details
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>WithdrawalQuota details</returns>
        Task<WithdrawalQuota> GetWithdrawalQuota(string symbol);

        /// <summary>
        /// Get withdrawal details
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="address">Address to send to</param>
        /// <param name="memo">Address memo</param>
        /// <param name="amount">Quantity to send</param>
        /// <param name="inner">Internal withdrawal?</param>
        /// <param name="remark">Remarks of transaction</param>
        /// <returns>WithdrawalQuota details</returns>
        Task<string> Withdrawal(string symbol, string address, decimal amount, string memo = "", bool inner = false, string remark = "");

        /// <summary>
        /// Cancel a withdrawal
        /// </summary>
        /// <param name="withdrawalId">Withdrawal Id to cancel</param>
        /// <returns>Withdrawal Id</returns>
        Task<string> CancelWithdrawal(string withdrawalId);

        #endregion Secure Endpoints

        #region Public Endpoints
        
        /// <summary>
        /// Check if the Exchange Repository is ready for trading
        /// </summary>
        /// <returns>Boolean of validation</returns>
        bool ValidateExchangeConfigured();

        /// <summary>
        /// Get available trading pairs
        /// </summary>
        /// <returns>Collection of Trading Pair Details</returns>
        Task<List<TradingPairDetail>> GetTradingPairDetails();

        /// <summary>
        /// Get Ticker for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>Ticker of pair</returns>
        Task<Ticker> GetTicker(string pair);

        /// <summary>
        /// Get order book for a pair, 100 depth bid & ask
        /// Fastest order book available in REST
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>OrderBook data</returns>
        Task<OrderBookL2> GetPartOrderBook(string pair);

        /// <summary>
        /// Get order book for a pair, full depth
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>OrderBook data</returns>
        Task<OrderBookL2> GetFullOrderBook(string pair);

        /// <summary>
        /// Returns entire order book
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>Detailed OrderBook data</returns>
        Task<OrderBookL3> GetEntireOrderBook(string pair);

        /// <summary>
        /// Get latest trades
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>Collection of TradeHistory</returns>
        Task<List<TradeHistory>> GetTradeHistory(string pair);

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="startAt">Starting date</param>
        /// <param name="endAt">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Collection of candlesticks</returns>
        Task<List<Candlestick>> GetCandlestick(string pair, long startAt, long endAt, Interval interval);

        /// <summary>
        /// Get All tickers for exchange
        /// </summary>
        /// <returns>Collection of TradingPairStats objects</returns>
        Task<List<TradingPairStats>> GetAllTickers();

        /// <summary>
        /// Get 24 hour stats for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>TradingPairStats object</returns>
        Task<TradingPairStats> Get24HrStats(string pair);

        /// <summary>
        /// Get the transaction currency for the entire trading market.
        /// </summary>
        /// <returns>Collection of currencies</returns>
        Task<List<string>> GetMarkets();

        /// <summary>
        /// Get known currencies
        /// </summary>
        /// <returns>Collection of Currency objects</returns>
        Task<List<Currency>> GetCurrencies();

        /// <summary>
        /// Get currency detail
        /// </summary>
        /// <param name="symbol">Currency symbol</param>
        /// <returns>CurrencyDetail objects</returns>
        Task<CurrencyDetail> GetCurrency(string symbol);

        /// <summary>
        /// Get fiat price for currency
        /// </summary>
        /// <param name="baseCurrency">Base currency (USD, EUR) Default USD</param>
        /// <param name="currencies">Comma separated list of currencies to limit out put (BTC, ETH) default all</param>
        /// <returns>Currencies and fiat prices</returns>
        Task<Dictionary<string, decimal>> GetFiatPrice(string baseCurrency = null, string currencies = null);

        /// <summary>
        /// Get server time from KuCoin
        /// </summary>
        /// <returns>Unix server time</returns>
        Task<long> GetServerTime();

        #endregion Public Endpoints
    }
}