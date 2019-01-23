// -----------------------------------------------------------------------------
// <copyright file="IKuCoinDotNetExtension" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/21/2019 7:50:14 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net
{
    #region Usings

    using DateTimeHelpers;
    using KuCoinApi.Net.Core;
    using KuCoinApi.Net.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    #endregion Usings

    public static class IKuCoinDotNetExtension
    {
        #region Secure Endpoints

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns>Balance collection</returns>
        public static async Task<List<Balance>> GetBalances(this IKuCoinDotNet service)
        {
            return await service.GetBalances(string.Empty, null);
        }
        
        /// <summary>
        /// Get all account balances
        /// </summary>
        /// <param name="hideZeroBalance">Hide zero balance coins</param>
        /// <returns>Balance collection</returns>
        public static async Task<List<Balance>> GetBalances(this IKuCoinDotNet service, bool hideZeroBalance = false)
        {
            var balances = await service.GetBalances(string.Empty, null);

            return hideZeroBalance
                ? balances.Where(b => b.Total > 0).Distinct().ToList()
                : balances.Distinct().ToList();
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Balance collection</returns>
        public static async Task<List<Balance>> GetBalances(this IKuCoinDotNet service, string symbol)
        {
            return await service.GetBalances(symbol, null);
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="type">Account type</param>
        /// <returns>Balance collection</returns>
        public static async Task<List<Balance>> GetBalances(this IKuCoinDotNet service, AccountType type)
        {
            return await service.GetBalances(string.Empty, type);
        }

        /// <summary>
        /// Get account history
        /// </summary>
        /// <param name="accountId">id of account</param>
        /// <param name="startAt">Start time</param>
        /// <param name="endAt">End time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged response of account history</returns>
        public static async Task<PagedResponse<List<AccountAction>>> GetAccountHistory(this IKuCoinDotNet service, string accountId, DateTime startAt, DateTime endAt, int page = 0, int pageSize = 0)
        {
            if (startAt >= endAt)
            {
                throw new Exception("Start date cannot be >= End date.");
            }
            var _dtHelper = new DateTimeHelper();
            var startNonce = _dtHelper.LocalToUnixTime(startAt);
            var endNonce = _dtHelper.LocalToUnixTime(endAt);

            return await service.GetAccountHistory(accountId, startNonce, endNonce, page, pageSize);
        }

        /// <summary>
        /// Transfer funds between accounts
        /// </summary>
        /// <param name="fromId">Account Id Payer</param>
        /// <param name="toId">Account Id Receiver</param>
        /// <param name="amount">Amount to transfer</param>
        /// <returns>Id of funds transfer order</returns>
        public static async Task<string> InnerTransfer(this IKuCoinDotNet service, string fromId, string toId, decimal amount)
        {
            var clientOid = Guid.NewGuid().ToString().Replace("-", "");

            return await service.InnerTransfer(clientOid, fromId, toId, amount);
        }

        /// <summary>
        /// Place a limit order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Side of trade</param>
        /// <param name="price">Price of trade</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <returns>String of order id</returns>
        public static async Task<string> PlaceLimitOrder(this IKuCoinDotNet service, string pair, Side side, decimal price, decimal quantity)
        {
            var clientOid = Guid.NewGuid().ToString().Replace("-", "");
            var body = new SortedDictionary<string, object>();
            body.Add("clientOid", clientOid);
            body.Add("symbol", pair);
            body.Add("price", price);
            body.Add("side", side.ToString().ToLower());
            body.Add("size", quantity);
            body.Add("type", OrderType.LIMIT.ToString().ToLower());

            return await service.PlaceOrder(body);
        }

        /// <summary>
        /// Place a limit order
        /// </summary>
        /// <param name="parms">Limit Order Parameters</param>
        /// <returns>String of order id</returns>
        public static async Task<string> PlaceLimitOrder(this IKuCoinDotNet service, LimitOrderParams parms)
        {
            var body = new SortedDictionary<string, object>();
            if (parms.CancelAfter > 0)
                body.Add("cancelAfter", parms.CancelAfter);
            if (string.IsNullOrEmpty(parms.ClientOid))
                parms.ClientOid = Guid.NewGuid().ToString();
            body.Add("clientOid", parms.ClientOid);
            body.Add("symbol", parms.Pair);
            if (parms.PostOnly)
                body.Add("postOnly", parms.PostOnly);
            body.Add("price", parms.Price);
            if (!string.IsNullOrEmpty(parms.Remark))
                body.Add("remark", parms.Remark);
            if (parms.SelfTradeProtect != null)
                body.Add("stp", parms.SelfTradeProtect.ToString());
            body.Add("side", parms.Side.ToString().ToLower());
            body.Add("size", parms.Size);
            if (parms.TimeInForce != null)
                body.Add("timeInForce", parms.TimeInForce);
            body.Add("type", parms.Type.ToString().ToLower());

            return await service.PlaceOrder(body);
        }

        /// <summary>
        /// Place a market order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Side of trade</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <returns>String of order id</returns>
        public static async Task<string> PlaceMarketOrder(this IKuCoinDotNet service, string pair, Side side, decimal quantity)
        {
            var clientOid = Guid.NewGuid().ToString().Replace("-", "");
            var body = new SortedDictionary<string, object>();
            body.Add("clientOid", clientOid);
            body.Add("symbol", pair);
            body.Add("side", side.ToString().ToLower());
            body.Add("size", quantity);
            body.Add("type", OrderType.MARKET.ToString().ToLower());

            return await service.PlaceOrder(body);
        }

        /// <summary>
        /// Place a market order
        /// </summary>
        /// <param name="parms">Market Order Parameters</param>
        /// <returns>String of order id</returns>
        public static async Task<string> PlaceMarketOrder(this IKuCoinDotNet service, MarketOrderParams parms)
        {
            var body = new SortedDictionary<string, object>();
            if (string.IsNullOrEmpty(parms.ClientOid))
                parms.ClientOid = Guid.NewGuid().ToString();
            if (parms.Funds > 0)
                body.Add("funds", parms.Funds);
            body.Add("clientOid", parms.ClientOid);
            body.Add("symbol", parms.Pair);
            if (!string.IsNullOrEmpty(parms.Remark))
                body.Add("remark", parms.Remark);
            if (parms.SelfTradeProtect != null)
                body.Add("stp", parms.SelfTradeProtect.ToString());
            body.Add("side", parms.Side.ToString().ToLower());
            body.Add("size", parms.Size);
            body.Add("type", parms.Type.ToString().ToLower());

            return await service.PlaceOrder(body);
        }

        /// <summary>
        /// Place a stop order
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Side of trade</param>
        /// <param name="price">Price of trade</param>
        /// <param name="quantity">Quantity to trade</param>
        /// <param name="stopPrice">Stop price of trade</param>
        /// <param name="stopType">Stop order type</param>
        /// <returns>String of order id</returns>
        public static async Task<string> PlaceStopOrder(this IKuCoinDotNet service, string pair, Side side, decimal price, decimal quantity, decimal stopPrice, StopType stopType)
        {
            var clientOid = Guid.NewGuid().ToString().Replace("-", "");
            var body = new SortedDictionary<string, object>();
            body.Add("clientOid", clientOid);
            body.Add("symbol", pair);
            body.Add("side", side.ToString().ToLower());
            body.Add("size", quantity);
            body.Add("stop", stopType.ToString().ToLower());
            body.Add("stopPrice", stopPrice);
            body.Add("price", price);
            body.Add("type", OrderType.LIMIT.ToString().ToLower());

            return await service.PlaceOrder(body);
        }

        /// <summary>
        /// Place a stop order
        /// </summary>
        /// <param name="parms">Stop Limit Order Parameters</param>
        /// <returns>String of order id</returns>
        public static async Task<string> PlaceStopOrder(this IKuCoinDotNet service, StopLimitOrderParams parms)
        {
            var body = new SortedDictionary<string, object>();
            if (parms.CancelAfter > 0)
                body.Add("cancelAfter", parms.CancelAfter);
            if (string.IsNullOrEmpty(parms.ClientOid))
                parms.ClientOid = Guid.NewGuid().ToString();
            body.Add("clientOid", parms.ClientOid);
            body.Add("symbol", parms.Pair);
            if (parms.PostOnly)
                body.Add("postOnly", parms.PostOnly);
            body.Add("price", parms.Price);
            if (!string.IsNullOrEmpty(parms.Remark))
                body.Add("remark", parms.Remark);
            if (parms.SelfTradeProtect != null)
                body.Add("stp", parms.SelfTradeProtect.ToString());
            body.Add("side", parms.Side.ToString().ToLower());
            body.Add("size", parms.Size);
            body.Add("stop", parms.Stop.ToString().ToLower());
            body.Add("stopPrice", parms.StopPrice);
            if (parms.TimeInForce != null)
                body.Add("timeInForce", parms.TimeInForce.ToString());
            body.Add("type", parms.Type.ToString().ToLower());

            return await service.PlaceOrder(body);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public static async Task<PagedResponse<List<Order>>> GetOrders(this IKuCoinDotNet service, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public static async Task<PagedResponse<List<Order>>> GetOrders(this IKuCoinDotNet service, string pair, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(pair: pair, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public static async Task<PagedResponse<List<Order>>> GetOrders(this IKuCoinDotNet service, string pair, Side side, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(pair: pair, side: side, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="status">Order status</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public static async Task<PagedResponse<List<Order>>> GetOrders(this IKuCoinDotNet service, string pair, OrderStatus status, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(pair: pair, status: status, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="status">Order status</param>
        /// <param name="side">Trade side</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public static async Task<PagedResponse<List<Order>>> GetOrders(this IKuCoinDotNet service, string pair, Side side, OrderStatus status, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(pair: pair, side: side, status: status, page: page, pageSize: pageSize);
        }

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
        public static async Task<PagedResponse<List<Order>>> GetOrders(this IKuCoinDotNet service, string pair, OrderStatus? status, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0)
        {
            var _dtHelper = new DateTimeHelper();
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await service.GetOrders(status, pair, side, type, startAt, endAt, page, pageSize);
        }

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
        public static async Task<PagedResponse<List<Order>>> GetOrders(this IKuCoinDotNet service, string pair, OrderStatus? status, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(status, pair, side, type, startAt, endAt, page, pageSize);
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public static async Task<PagedResponse<List<Order>>> GetOpenOrders(this IKuCoinDotNet service, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(status: OrderStatus.ACTIVE, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public static async Task<PagedResponse<List<Order>>> GetOpenOrders(this IKuCoinDotNet service, string pair, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(status: OrderStatus.ACTIVE, pair: pair, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public static async Task<PagedResponse<List<Order>>> GetOpenOrders(this IKuCoinDotNet service, string pair, Side side, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(OrderStatus.ACTIVE, pair: pair, side: side, page: 0, pageSize: 0);
        }

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
        public static async Task<PagedResponse<List<Order>>> GetOpenOrders(this IKuCoinDotNet service, string pair, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0)
        {
            var _dtHelper = new DateTimeHelper();
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await service.GetOrders(OrderStatus.ACTIVE, pair, side, type, startAt, endAt, page, pageSize);
        }

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
        public static async Task<PagedResponse<List<Order>>> GetOpenOrders(this IKuCoinDotNet service, string pair, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0)
        {
            return await service.GetOrders(OrderStatus.ACTIVE, pair, side, type, startAt, endAt, page, pageSize);
        }

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <returns>Page collection of Fills</returns>
        public static async Task<PagedResponse<List<Fill>>> GetFills(this IKuCoinDotNet service, int page = 0, int pageSize = 0)
        {
            return await service.GetFills(page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Page collection of Fills</returns>
        public static async Task<PagedResponse<List<Fill>>> GetFillsForOrder(this IKuCoinDotNet service, string orderId, int page = 0, int pageSize = 0)
        {
            return await service.GetFills(orderId: orderId, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Page collection of Fills</returns>
        public static async Task<PagedResponse<List<Fill>>> GetFillsForPair(this IKuCoinDotNet service, string pair, int page = 0, int pageSize = 0)
        {
            return await service.GetFills(pair: pair, page: page, pageSize: pageSize);
        }

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
        public static async Task<PagedResponse<List<Fill>>> GetFills(this IKuCoinDotNet service, string orderId, string pair, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0)
        {
            var _dtHelper = new DateTimeHelper();
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await service.GetFills(orderId, pair, side, type, startAt, endAt, page, pageSize);
        }

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
        public static async Task<PagedResponse<List<Fill>>> GetFills(this IKuCoinDotNet service, string orderId, string pair, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0)
        {
            return await service.GetFills(orderId, pair, side, type, startAt, endAt, page, pageSize);
        }

        /// <summary>
        /// Get deposit history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        public static async Task<PagedResponse<List<Deposit>>> GetDepositHistory(this IKuCoinDotNet service, string symbol, int page = 0, int pageSize = 0)
        {
            return await service.GetDepositHistory(symbol: symbol, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get deposit history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="status">Deposit status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        public static async Task<PagedResponse<List<Deposit>>> GetDepositHistory(this IKuCoinDotNet service, string symbol, DepositStatus status, int page = 0, int pageSize = 0)
        {
            return await service.GetDepositHistory(symbol: symbol, status: status, page: page, pageSize: pageSize);
        }

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
        public static async Task<PagedResponse<List<Deposit>>> GetDepositHistory(this IKuCoinDotNet service, string symbol = null, DateTime? startDate = null, DateTime? endDate = null, DepositStatus? status = null, int page = 0, int pageSize = 0)
        {
            var _dtHelper = new DateTimeHelper();
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await service.GetDepositHistory(symbol, startAt, endAt, status, page, pageSize);
        }

        /// <summary>
        /// Get withdrawal history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        public static async Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(this IKuCoinDotNet service, string symbol, int page = 0, int pageSize = 0)
        {
            return await service.GetWithdrawalHistory(symbol: symbol, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get withdrawal history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="status">Withdrawal status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        public static async Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(this IKuCoinDotNet service, string symbol, DepositStatus status, int page = 0, int pageSize = 0)
        {
            return await service.GetWithdrawalHistory(symbol: symbol, status: status, page: page, pageSize: pageSize);
        }

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
        public static async Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(this IKuCoinDotNet service, string symbol = null, DateTime? startDate = null, DateTime? endDate = null, WithdrawalStatus? status = null, int page = 0, int pageSize = 0)
        {
            var _dtHelper = new DateTimeHelper();
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await service.GetWithdrawalHistory(symbol, startAt, endAt, status, page, pageSize);
        }

        #endregion Secure Endpoints

        #region Public Endpoints

        /// <summary>
        /// Get current markets on the exchange
        /// </summary>
        /// <param name="trading">Currently trading</param>
        /// <returns>Collection of trading pairs</returns>
        public static async Task<List<string>> GetMarkets(this IKuCoinDotNet service, bool trading = true)
        {
            var details = await service.GetTradingPairDetails();

            return details.Select(d => d.Pair).ToList();
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        public static async Task<List<Candlestick>> GetCandlestick(this IKuCoinDotNet service, string pair, Interval interval, int stickCount)
        {
            var _dtHelper = new DateTimeHelper();
            var _helper = new Helper();
            var endAt = _dtHelper.UTCEndOfMinuteToUnixTime();
            var startAt = _helper.GetFromUnixTime(endAt, interval, (stickCount + 2));

            return await service.GetCandlestick(pair, startAt, endAt, interval);
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="endAt">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        public static async Task<List<Candlestick>> GetCandlestick(this IKuCoinDotNet service, string pair, long endAt, Interval interval, int stickCount)
        {
            var _dtHelper = new DateTimeHelper();
            var _helper = new Helper();
            var startAt = _helper.GetFromUnixTime(endAt, interval, (stickCount + 2));

            return await service.GetCandlestick(pair, startAt, endAt, interval);
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="endDate">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        public static async Task<List<Candlestick>> GetCandlestick(this IKuCoinDotNet service, string pair, DateTime endDate, Interval interval, int stickCount)
        {
            var _dtHelper = new DateTimeHelper();
            var _helper = new Helper();
            var endAt = _dtHelper.LocalToUnixTime(endDate);
            var startAt = _helper.GetFromUnixTime(endAt, interval, (stickCount + 2));

            return await service.GetCandlestick(pair, startAt, endAt, interval);
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="startDate">Starting date</param>
        /// <param name="endDate">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Collection of candlesticks</returns>
        public static async Task<List<Candlestick>> GetCandlestick(this IKuCoinDotNet service, string pair, DateTime startDate, DateTime endDate, Interval interval)
        {
            var _dtHelper = new DateTimeHelper();
            var startAt = _dtHelper.LocalToUnixTime(startDate);
            var endAt = _dtHelper.LocalToUnixTime(endDate);

            return await service.GetCandlestick(pair, startAt, endAt, interval);
        }

        #endregion Public Endpoints
    }
}