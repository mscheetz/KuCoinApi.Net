// -----------------------------------------------------------------------------
// <copyright file="KuCoinDotNet" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/21/2019 7:49:44 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net
{
    #region Usings

    using DateTimeHelpers;
    using FileRepository;
    using KuCoinApi.Net.Core;
    using KuCoinApi.Net.Data;
    using KuCoinApi.Net.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    #endregion Usings

    public class KuCoinDotNet : RepositoryBase, IKuCoinDotNet
    {
        #region Properties

        private ApiInformation _apiInfo;
        private DateTimeHelper _dtHelper;
        private Helper _helper;
        private bool _systemTimetamp = false;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructor for non-signed endpoints
        /// </summary>
        /// <param name="sandbox">Use sandbox? (default = false)</param>
        public KuCoinDotNet(bool sandbox = false) : base(sandbox)
        {
            LoadRepository();
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        /// <param name="apiPassword">Api password</param>
        /// <param name="sandbox">Use sandbox? (default = false)</param>
        public KuCoinDotNet(string apiKey, string apiSecret, string apiPassword, bool sandbox = false) : this(new ApiInformation { ApiKey = apiKey, ApiSecret = apiSecret, ApiPassword = apiPassword }, sandbox)
        {
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="configPath">String of path to configuration file</param>
        /// <param name="sandbox">Use sandbox? (default = false)</param>
        public KuCoinDotNet(string configPath, bool sandbox = false) : base(sandbox)
        {
            IFileRepository _fileRepo = new FileRepository();

            if (_fileRepo.FileExists(configPath))
            {
                _apiInfo = _fileRepo.GetDataFromFile<ApiInformation>(configPath);
                base.SetApiKey(_apiInfo);
                LoadRepository(_apiInfo);
            }
            else
            {
                throw new Exception("Config file not found");
            }
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="apiInformation">Api Key information</param>
        /// <param name="sandbox">Use sandbox? (default = false)</param>
        public KuCoinDotNet(ApiInformation apiInformation, bool sandbox = false) : base(apiInformation, sandbox)
        {
            _apiInfo = apiInformation;
            LoadRepository(apiInformation);
        }

        private void LoadRepository(ApiInformation apiInformation)
        {
            LoadRepository(apiInformation.ApiKey, apiInformation.ApiSecret, apiInformation.ApiPassword);
        }

        /// <summary>
        /// Load repository
        /// </summary>
        /// <param name="key">Api key value (default = "")</param>
        /// <param name="secret">Api secret value (default = "")</param>
        /// <param name="password">Api password value (default = "")</param>
        private void LoadRepository(string key = "", string secret = "", string password = "")
        {
            _dtHelper = new DateTimeHelper();
            _helper = new Helper();
            _apiInfo = new ApiInformation
            {
                ApiKey = key,
                ApiSecret = secret,
                ApiPassword = password
            };
            _systemTimetamp = TimestampCompare();
            base.SetTimestamp(_systemTimetamp);
        }

        #endregion Constructor

        #region Secure Endpoints
        
        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="type">Account type</param>
        /// <returns>Balance collection</returns>
        public async Task<List<Balance>> GetBalances(string symbol, AccountType? type)
        {
            var endpoint = "/api/v1/accounts";
            var parms = new SortedDictionary<string, object>();

            if (!string.IsNullOrEmpty(symbol))
                parms.Add("currency", symbol);
            if (type != null)
                parms.Add("type", type.ToString().ToLower());

            var queryString = parms.Count > 0 ? $"?{_helper.SortedDictionaryToString(parms)}" : string.Empty;

            endpoint = endpoint + queryString;

            return await Get<List<Balance>>(endpoint, true);
        }

        /// <summary>
        /// Get account balance of an account
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <returns>Balance object</returns>
        public async Task<Balance> GetBalance(string accountId)
        {
            var endpoint = $"/api/v1/accounts/{accountId}";

            return await Get<Balance>(endpoint, true);
        }

        /// <summary>
        /// Create an account
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="type">Type of account</param>
        /// <returns>Id of new account</returns>
        public async Task<string> CreateAccount(string symbol, AccountType type)
        {
            var endpoint = "/api/v1/accounts";

            var body = new SortedDictionary<string, object>();
            body.Add("currency", symbol);
            body.Add("type", type.ToString().ToLower());

            var response = await Post<IdResponse>(endpoint, body);

            return response.Id;
        }

        /// <summary>
        /// Get account history
        /// </summary>
        /// <param name="accountId">id of account</param>
        /// <param name="startAt">Unix start time</param>
        /// <param name="endAt">Unix end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged response of account history</returns>
        public async Task<PagedResponse<List<AccountAction>>> GetAccountHistory(string accountId, long startAt, long endAt, int page = 0, int pageSize = 0)
        {
            var endpoint = $"/api/v1/accounts/{accountId}/ledgers";
            
            if ((startAt > 0 && endAt > 0) && startAt >= endAt)
            {
                throw new Exception("Start date cannot be >= End date.");
            }
            var parms = new Dictionary<string, object>();
            if (startAt > 0)
                parms.Add("startAt", startAt);
            if (endAt > 0)
                parms.Add("endAt", endAt);
            if (page == 0)
                page = 1;

            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;

            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<AccountAction>>>(endpoint, true);
        }

        /// <summary>
        /// Get holds on an account
        /// </summary>
        /// <param name="accountId">id of account</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged response of account holds</returns>
        public async Task<PagedResponse<List<AccountHold>>> GetHolds(string accountId, int page = 0, int pageSize = 0)
        {
            var endpoint = $"/api/v1/accounts/{accountId}/holds";

            var parms = new Dictionary<string, object>();
            if (page == 0)
                page = 1;
            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;

            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"&{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<AccountHold>>>(endpoint, true);
        }

        /// <summary>
        /// Transfer funds between accounts
        /// </summary>
        /// <param name="clientOid">Request Id</param>
        /// <param name="fromId">Account Id Payer</param>
        /// <param name="toId">Account Id Receiver</param>
        /// <param name="amount">Amount to transfer</param>
        /// <returns>Id of funds transfer order</returns>
        public async Task<string> InnerTransfer(string clientOid, string fromId, string toId, decimal amount)
        {
            var endpoint = "/api/v1/accounts/inner-transfer";

            var body = new SortedDictionary<string, object>();
            body.Add("clientOid", clientOid);
            body.Add("payAccountId", fromId);
            body.Add("recAccountId", toId);
            body.Add("amount", amount);

            var response = await Post<OrderResponse>(endpoint, body);

            return response.OrderId;
        }

        /// <summary>
        /// Place an order
        /// </summary>
        /// <param name="body">Body of order message</param>
        /// <returns>String of order id</returns>
        public async Task<string> PlaceOrder(SortedDictionary<string, object> body)
        {
            var endpoint = "/api/v1/orders";

            var response = await Post<OrderResponse>(endpoint, body);

            return response.OrderId;
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="orderId">Id of order to cancel</param>
        /// <returns>Id of order canceled</returns>
        public async Task<string> CancelOrder(string orderId)
        {
            var endpoint = $"/api/v1/orders/{orderId}";

            var result = await Delete<Dictionary<string, string[]>>(endpoint);

            return result["cancelledOrderIds"][0];
        }

        /// <summary>
        /// Cancel all open orders
        /// </summary>
        /// <returns>Collection of canceled order Ids</returns>
        public async Task<List<string>> CancelAllOrders()
        {
            var endpoint = $"/api/v1/orders";

            var result = await Delete<Dictionary<string, string[]>>(endpoint);

            return result["cancelledOrderIds"].ToList();
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
        public async Task<PagedResponse<List<Order>>> GetOrders(OrderStatus? status = null, string pair = null, Side? side = null, OrderType? type = null, long startAt = 0, long endAt = 0, int page = 1, int pageSize = 50)
        {
            if ((startAt > 0 && endAt > 0) && startAt >= endAt)
            {
                throw new Exception("Start time cannot be on or after End time");
            }

            var endpoint = $"/api/v1/orders";

            var parms = new Dictionary<string, object>();
            if (status != null)
                parms.Add("status", status.ToString());
            if (!string.IsNullOrEmpty(pair))
                parms.Add("symbol", pair);
            if (side != null)
                parms.Add("side", side.ToString().ToLower());
            if (type != null)
                parms.Add("type", type.ToString().ToLower());
            if (startAt > 0)
                parms.Add("startAt", startAt);
            if (endAt > 0)
                parms.Add("endAt", endAt);
            if (page == 0)
                page = 1;
            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;
            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<Order>>>(endpoint, true);
        }

        /// <summary>
        /// Get an order
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns>An Order</returns>
        public async Task<Order> GetOrder(string orderId)
        {
            var endpoint = $"/api/v1/orders/{orderId}";

            return await Get<Order>(endpoint, true);
        }

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
        public async Task<PagedResponse<List<HistoricOrder>>> GetHistoricOrders(string pair = null, Side? side = null, long startAt = 0, long endAt = 0, int page = 1, int pageSize = 50)
        {
            if ((startAt > 0 && endAt > 0) && startAt >= endAt)
            {
                throw new Exception("Start time cannot be on or after End time");
            }

            var endpoint = $"/api/v1/hist-orders";

            var parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(pair))
                parms.Add("symbol", pair);
            if (side != null)
                parms.Add("side", side.ToString().ToLower());
            if (startAt > 0)
                parms.Add("startAt", startAt);
            if (endAt > 0)
                parms.Add("endAt", endAt);
            if (page == 0)
                page = 1;
            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;
            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<HistoricOrder>>>(endpoint, true);
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
        public async Task<PagedResponse<List<Fill>>> GetFills(string orderId = null, string pair = null, Side? side = null, OrderType? type = null, long startAt = 0, long endAt = 0, int page = 0, int pageSize = 0)
        {
            if ((startAt > 0 && endAt > 0) && startAt >= endAt)
            {
                throw new Exception("Start time cannot be on or after End time");
            }

            var endpoint = $"/api/v1/fills";

            var parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(orderId))
                parms.Add("orderId", orderId);
            if (!string.IsNullOrEmpty(pair))
                parms.Add("symbol", pair);
            if (side != null)
                parms.Add("side", side.ToString().ToLower());
            if (type != null)
                parms.Add("type", type.ToString().ToLower());
            if (startAt > 0)
                parms.Add("startAt", startAt);
            if (endAt > 0)
                parms.Add("endAt", endAt);
            if (page == 0)
                page = 1;
            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;
            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<Fill>>>(endpoint, true);
        }

        /// <summary>
        /// Create Deposit address for a currency
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>New Deposit Address</returns>
        public async Task<DepositAddress> CreateDepositAddress(string symbol)
        {
            var endpoint = $"/api/v1/deposit-addresses";

            var body = new SortedDictionary<string, object>();
            body.Add("currency", symbol);

            return await Post<DepositAddress>(endpoint, body);
        }

        /// <summary>
        /// Get Deposit address for a currency
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Deposit Address</returns>
        public async Task<DepositAddress> GetDepositAddress(string symbol)
        {
            var endpoint = $"/api/v1/deposit-addresses?currency={symbol}";

            return await Get<DepositAddress>(endpoint, true);
        }

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
        public async Task<PagedResponse<List<Deposit>>> GetDepositHistory(string symbol = null, long startAt = 0, long endAt = 0, DepositStatus? status = null, int page = 0, int pageSize = 0)
        {
            var endpoint = $"/api/v1/deposits";

            var parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(symbol))
                parms.Add("currency", symbol);
            if (startAt > 0)
                parms.Add("startAt", startAt);
            if (endAt > 0)
                parms.Add("endAt", endAt);
            if (status != null)
                parms.Add("status", status.ToString());
            if (page == 0)
                page = 1;
            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;
            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<Deposit>>>(endpoint, true);
        }

        /// <summary>
        /// Get historic deposit history (KuCoin v1)
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="startAt">Start date</param>
        /// <param name="endAt">End date</param>
        /// <param name="status">Deposit status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        public async Task<PagedResponse<List<Deposit>>> GetHistoricDeposits(string symbol = null, long startAt = 0, long endAt = 0, DepositStatus? status = null, int page = 0, int pageSize = 0)
        {
            var endpoint = $"/api/v1/hist-deposits";

            var parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(symbol))
                parms.Add("currency", symbol);
            if (startAt > 0)
                parms.Add("startAt", startAt);
            if (endAt > 0)
                parms.Add("endAt", endAt);
            if (status != null)
                parms.Add("status", status.ToString());
            if (page == 0)
                page = 1;
            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;
            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<Deposit>>>(endpoint, true);
        }

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
        public async Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(string symbol = null, long startAt = 0, long endAt = 0, WithdrawalStatus? status = null, int page = 0, int pageSize = 0)
        {
            var endpoint = $"/api/v1/withdrawals";

            var parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(symbol))
                parms.Add("currency", symbol);
            if (startAt > 0)
                parms.Add("startAt", startAt);
            if (endAt > 0)
                parms.Add("endAt", endAt);
            if (status != null)
                parms.Add("status", status.ToString());
            if (page == 0)
                page = 1;
            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;
            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<Withdrawal>>>(endpoint, true);
        }

        /// <summary>
        /// Get historic deposit history (KuCoin v1)
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="startAt">Start date</param>
        /// <param name="endAt">End date</param>
        /// <param name="status">Withdrawal status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        public async Task<PagedResponse<List<Withdrawal>>> GetHistoricWithdrawals(string symbol = null, long startAt = 0, long endAt = 0, WithdrawalStatus? status = null, int page = 0, int pageSize = 0)
        {
            var endpoint = $"/api/v1/hist-withdrawals";

            var parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(symbol))
                parms.Add("currency", symbol);
            if (startAt > 0)
                parms.Add("startAt", startAt);
            if (endAt > 0)
                parms.Add("endAt", endAt);
            if (status != null)
                parms.Add("status", status.ToString());
            if (page == 0)
                page = 1;
            parms.Add("currentPage", page);
            if (pageSize == 0)
                pageSize = 100;
            parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            endpoint += queryString;

            return await Get<PagedResponse<List<Withdrawal>>>(endpoint, true);
        }

        /// <summary>
        /// Get withdrawal details
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>WithdrawalQuota details</returns>
        public async Task<WithdrawalQuota> GetWithdrawalQuota(string symbol)
        {
            var endpoint = $"/api/v1/withdrawals/quota?currency={symbol}";

            return await Get<WithdrawalQuota>(endpoint, true);
        }

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
        public async Task<string> Withdrawal(string symbol, string address, decimal amount, string memo = "", bool inner = false, string remark = "")
        {
            var endpoint = $"/api/v1/withdrawals";
            
            var body = new SortedDictionary<string, object>();
            body.Add("currency", symbol);
            body.Add("address", address);
            body.Add("memo", memo);
            body.Add("amount", amount);
            body.Add("isInner", inner);
            body.Add("remark", remark);

            return await Post<string>(endpoint, body);
        }

        /// <summary>
        /// Cancel a withdrawal
        /// </summary>
        /// <param name="withdrawalId">Withdrawal Id to cancel</param>
        /// <returns>Withdrawal Id</returns>
        public async Task<string> CancelWithdrawal(string withdrawalId)
        {
            var endpoint = $"/api/v1/withdrawals/{withdrawalId}";

            return await Delete<string>(endpoint);
        }

        #endregion Secure Endpoints

        #region Public Endpoints

        /// <summary>
        /// Get available trading pairs
        /// </summary>
        /// <returns>Collection of Trading Pair Details</returns>
        public async Task<List<TradingPairDetail>> GetTradingPairDetails()
        {
            var endpoint = "/api/v1/symbols";

            return await Get<List<TradingPairDetail>>(endpoint, false);
        }

        /// <summary>
        /// Get Ticker for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>Ticker of pair</returns>
        public async Task<Ticker> GetTicker(string pair)
        {
            var endpoint = $"/api/v1/market/orderbook/level1?symbol={pair}";

            return await Get<Ticker>(endpoint, false);
        }

        /// <summary>
        /// Get order book for a pair, 100 depth bid & ask
        /// Fastest order book available in REST
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>OrderBook data</returns>
        public async Task<OrderBookL2> GetPartOrderBook(string pair)
        {
            var endpoint = $"/api/v1/market/orderbook/level2_100?symbol={pair}";

            return await Get<OrderBookL2>(endpoint, false);
        }

        /// <summary>
        /// Get order book for a pair, full depth
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>OrderBook data</returns>
        public async Task<OrderBookL2> GetFullOrderBook(string pair)
        {
            var endpoint = $"/api/v1/market/orderbook/level2?symbol={pair}";

            return await Get<OrderBookL2>(endpoint, false);
        }

        /// <summary>
        /// Returns entire order book
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>Detailed OrderBook data</returns>
        public async Task<OrderBookL3> GetEntireOrderBook(string pair)
        {
            var endpoint = $"/api/v1/market/orderbook/level3?symbol={pair}";

            return await Get<OrderBookL3>(endpoint, false);
        }

        /// <summary>
        /// Get latest trades
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>Collection of TradeHistory</returns>
        public async Task<List<TradeHistory>> GetTradeHistory(string pair)
        {
            var endpoint = $"/api/v1/market/histories?symbol={pair}";

            return await Get<List<TradeHistory>>(endpoint, false);
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="startAt">Starting date</param>
        /// <param name="endAt">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Collection of candlesticks</returns>
        public async Task<List<Candlestick>> GetCandlestick(string pair, long startAt, long endAt, Interval interval)
        {
            if ((startAt > 0 && endAt > 0) && startAt >= endAt)
            {
                throw new Exception("Start time cannot be >= end time.");
            }

            var type = _helper.GetEnumDescription(interval);

            var endpoint = $"/api/v1/market/candles?symbol={pair}&startAt={startAt}&endAt={endAt}&type={type}";

            return await Get<List<Candlestick>>(endpoint, false);
        }

        /// <summary>
        /// Get All tickers for exchange
        /// </summary>
        /// <returns>Collection of TradingPairStats objects</returns>
        public async Task<List<TradingPairStats>> GetAllTickers()
        {
            var endpoint = $"/api/v1/market/allTickers";

            var result =  await Get<AllTickerResponse>(endpoint, false);

            return result.Tickers;
        }

        /// <summary>
        /// Get 24 hour stats for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>TradingPairStats object</returns>
        public async Task<TradingPairStats> Get24HrStats(string pair)
        {
            var endpoint = $"/api/v1/market/stats?symbol={pair}";

            return await Get<TradingPairStats>(endpoint, false);
        }

        /// <summary>
        /// Get the transaction currency for the entire trading market.
        /// </summary>
        /// <returns>Collection of currencies</returns>
        public async Task<List<string>> GetMarkets()
        {
            var endpoint = $"/api/v1/markets";

            return await Get<List<string>>(endpoint, false);
        }

        /// <summary>
        /// Get known currencies
        /// </summary>
        /// <returns>Collection of Currency objects</returns>
        public async Task<List<Currency>> GetCurrencies()
        {
            var endpoint = "/api/v1/currencies";

            return await Get<List<Currency>>(endpoint, false);
        }

        /// <summary>
        /// Get currency detail
        /// </summary>
        /// <param name="symbol">Currency symbol</param>
        /// <returns>CurrencyDetail objects</returns>
        public async Task<CurrencyDetail> GetCurrency(string symbol)
        {
            var endpoint = $"/api/v1/currencies/{symbol}";

            return await Get<CurrencyDetail>(endpoint, false);
        }

        /// <summary>
        /// Get fiat price for currency
        /// </summary>
        /// <param name="baseCurrency">Base currency (USD, EUR) Default USD</param>
        /// <param name="currencies">Comma separated list of currencies to limit out put (BTC, ETH) default all</param>
        /// <returns>Currencies and fiat prices</returns>
        public async Task<Dictionary<string, decimal>> GetFiatPrice(string baseCurrency = null, string currencies = null)
        {
            var endpoint = $"/api/v1/prices";

            var parms = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(baseCurrency))
                parms.Add("base", baseCurrency);
            if (!string.IsNullOrEmpty(currencies))
                parms.Add("currencies", currencies);

            var queryString = parms.Count > 0 ? $"?{_helper.DictionaryToString(parms)}" : string.Empty;

            return await Get<Dictionary<string, decimal>>(endpoint, false);
        }

        /// <summary>
        /// Get server time from KuCoin
        /// </summary>
        /// <returns>Unix server time</returns>
        public async Task<long> GetServerTime()
        {
            var endpoint = "/api/v1/timestamp";

            var response = await base.GetRequest<long>(endpoint);

            return response;
        }

        /// <summary>
        /// Check if the Exchange Repository is ready for trading
        /// </summary>
        /// <returns>Boolean of validation</returns>
        public bool ValidateExchangeConfigured()
        {
            var ready = string.IsNullOrEmpty(_apiInfo.ApiKey) ? false : true;
            if (!ready)
                return false;

            return string.IsNullOrEmpty(_apiInfo.ApiSecret) || string.IsNullOrEmpty(_apiInfo.ApiPassword) ? false : true;
        }

        #endregion Public Endpoints

        #region Helpers

        /// <summary>
        /// Compare exchange and system unix timestamps
        /// </summary>
        /// <returns>True if difference less than 1000 MS, otherwise false</returns>
        private bool TimestampCompare()
        {
            var exchangeTS = GetTimestamp();
            var systemTS = _dtHelper.UTCtoUnixTimeMilliseconds();
            if (exchangeTS == systemTS || Math.Abs((decimal)exchangeTS - (decimal)systemTS) < 1000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get unix timestamp
        /// </summary>
        /// <returns>Long of timestamp</returns>
        private long GetTimestamp()
        {
            if (_systemTimetamp)
            {
                return _dtHelper.UTCtoUnixTimeMilliseconds();
            }
            else
            {
                return GetServerTime().Result;
            }
        }

        /// <summary>
        /// Delete request pre-processor
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="endpoint">Endpoint string</param>
        /// <returns>Async task of response</returns>
        public async Task<T> Delete<T>(string endpoint)
        {
            var timestamp = GetTimestamp();

            return await base.DeleteRequest<T>(endpoint, timestamp);
        }

        /// <summary>
        /// Get request pre-processor
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="endpoint">Endpoint string</param>
        /// <param name="secure">Secure call?</param>
        /// <returns>Async task of response</returns>
        public async Task<T> Get<T>(string endpoint, bool secure)
        {
            if (secure)
            {
                var timestamp = GetTimestamp();

                return await base.GetRequest<T>(endpoint, timestamp);
            }
            else
            {
                return await base.GetRequest<T>(endpoint);
            }
        }

        /// <summary>
        /// Post request pre-processor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint">Endpoint string</param>
        /// <param name="body">Request body data</param>
        /// <returns>Async task of response</returns>
        public async Task<T> Post<T>(string endpoint, SortedDictionary<string, object> body)
        {
            var timestamp = GetTimestamp();

            return await base.PostRequest<T>(endpoint, timestamp, body);
        }

        #endregion Helpers

    }
}