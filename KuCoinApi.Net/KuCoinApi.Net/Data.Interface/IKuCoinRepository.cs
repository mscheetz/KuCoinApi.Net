// -----------------------------------------------------------------------------
// <copyright file="IKuCoinRepository" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 8:40:42 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Data.Interface
{
    #region Usings

    using KuCoinApi.Net.Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    #endregion Usings

    public interface IKuCoinRepository
    {
        /// <summary>
        /// Check if the Exchange Repository is ready for trading
        /// </summary>
        /// <returns>Boolean of validation</returns>
        bool ValidateExchangeConfigured();
        
        #region Secure Endpoints

        /// <summary>
        /// Get all account balances
        /// </summary>
        /// <param name="hideZeroBalance">Hide zero balance coins</param>
        /// <returns>Balance collection</returns>
        Task<List<Balance>> GetBalances(bool hideZeroBalance = false);

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns>Balance collection</returns>
        Task<List<Balance>> GetBalances();

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Balance collection</returns>
        Task<List<Balance>> GetBalances(string symbol);

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="type">Account type</param>
        /// <returns>Balance collection</returns>
        Task<List<Balance>> GetBalances(AccountType type);

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="type">Account type</param>
        /// <returns>Balance collection</returns>
        Task<List<Balance>> GetBalances(string symbol, AccountType type);

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
        /// <param name="startAt">Start time</param>
        /// <param name="endAt">End time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged response of account history</returns>
        Task<PagedResponse<List<AccountAction>>> GetAccountHistory(string accountId, DateTime startAt, DateTime endAt, int page = 0, int pageSize = 0);

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
        /// <param name="fromId">Account Id Payer</param>
        /// <param name="toId">Account Id Receiver</param>
        /// <param name="amount">Amount to transfer</param>
        /// <returns>Id of funds transfer order</returns>
        Task<string> InnerTransfer(string fromId, string toId, decimal amount);

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
        /// Place a limit order
        /// </summary>
        /// <param name="parms">Limit Order Parameters</param>
        /// <returns>String of order id</returns>
        Task<string> PlaceLimitOrder(LimitOrderParams parms);

        /// <summary>
        /// Place a market order
        /// </summary>
        /// <param name="parms">Market Order Parameters</param>
        /// <returns>String of order id</returns>
        Task<string> PlaceMarketOrder(MarketOrderParams parms);

        /// <summary>
        /// Place a stop order
        /// </summary>
        /// <param name="parms">Stop Limit Order Parameters</param>
        /// <returns>String of order id</returns>
        Task<string> PlaceStopOrder(StopLimitOrderParams parms);

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
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOrders(int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOrders(string pair, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOrders(string pair, Side side, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="status">Order status</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOrders(string pair, OrderStatus status, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="status">Order status</param>
        /// <param name="side">Trade side</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOrders(string pair, Side side, OrderStatus status, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="status">Order status</param>
        /// <param name="side">Trade side</param>
        /// <param name="type">Order Type</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOrders(string pair, OrderStatus? status, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0);

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
        Task<PagedResponse<List<Order>>> GetOrders(string pair, OrderStatus? status, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOpenOrders(int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOpenOrders(string pair, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOpenOrders(string pair, Side side, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="type">Order Type</param>
        /// <param name="startDate">Start Date (Unix time)</param>
        /// <param name="endDate">End Date (Unix time)</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOpenOrders(string pair, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="type">Order Type</param>
        /// <param name="startAt">Start Date (Unix time)</param>
        /// <param name="endAt">End Date (Unix time)</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        Task<PagedResponse<List<Order>>> GetOpenOrders(string pair, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get an order
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns>An Order</returns>
        Task<Order> GetOrder(string orderId);

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <returns>Page collection of Fills</returns>
        Task<PagedResponse<List<Fill>>> GetFills(int page = 0, int pageSize = 0);

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Page collection of Fills</returns>
        Task<PagedResponse<List<Fill>>> GetFillsForOrder(string orderId, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Page collection of Fills</returns>
        Task<PagedResponse<List<Fill>>> GetFillsForPair(string pair, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="type">Order type</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Page collection of Fills</returns>
        Task<PagedResponse<List<Fill>>> GetFills(string orderId, string pair, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0);

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
        Task<PagedResponse<List<Fill>>> GetFills(string orderId, string pair, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0);

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
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        Task<PagedResponse<List<Deposit>>> GetDepositHistory(string symbol, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get deposit history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="status">Deposit status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        Task<PagedResponse<List<Deposit>>> GetDepositHistory(string symbol, DepositStatus status, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get deposit history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="status">Deposit status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        Task<PagedResponse<List<Deposit>>> GetDepositHistory(string symbol = null, DateTime? startDate = null, DateTime? endDate = null, DepositStatus? status = null, int page = 0, int pageSize = 0);

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
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(string symbol, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get withdrawal history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="status">Withdrawal status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(string symbol, DepositStatus status, int page = 0, int pageSize = 0);

        /// <summary>
        /// Get withdrawal history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="status">Withdrawal status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(string symbol = null, DateTime? startDate = null, DateTime? endDate = null, WithdrawalStatus? status = null, int page = 0, int pageSize = 0);

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
        Task<string> Withdrawal(string symbol, string address, string memo, decimal amount, bool inner, string remark);

        /// <summary>
        /// Cancel a withdrawal
        /// </summary>
        /// <param name="withdrawalId">Withdrawal Id to cancel</param>
        /// <returns>Withdrawal Id</returns>
        Task<string> CancelWithdrawal(string withdrawalId);

        #endregion Secure Endpoints

        #region Public Endpoints

        /// <summary>
        /// Get current markets on the exchange
        /// </summary>
        /// <param name="trading">Currently trading</param>
        /// <returns>Collection of trading pairs</returns>
        Task<List<string>> GetMarkets(bool trading = true);

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
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        Task<List<Candlestick>> GetCandlestick(string pair, Interval interval, int stickCount);

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="endAt">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        Task<List<Candlestick>> GetCandlestick(string pair, long endAt, Interval interval, int stickCount);

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="endDate">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        Task<List<Candlestick>> GetCandlestick(string pair, DateTime endDate, Interval interval, int stickCount);

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="startDate">Starting date</param>
        /// <param name="endDate">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Collection of candlesticks</returns>
        Task<List<Candlestick>> GetCandlestick(string pair, DateTime startDate, DateTime endDate, Interval interval);

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
        /// Get 24 hour stats for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>TradingPairStats object</returns>
        Task<TradingPairStats> Get24HrStats(string pair);

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
        /// Get server time from KuCoin
        /// </summary>
        /// <returns>Unix server time</returns>
        Task<long> GetServerTime();

        #endregion Public Endpoints
    }
}
