// -----------------------------------------------------------------------------
// <copyright file="KuCoinRepository" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 9:48:42 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Data
{
    #region Usings

    using DateTimeHelpers;
    using FileRepository;
    using KuCoinApi.Net.Core;
    using KuCoinApi.Net.Data.Interface;
    using KuCoinApi.Net.Entities;
    using Newtonsoft.Json;
    //using RESTApiAccess;
    //using RESTApiAccess.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    #endregion Usings

    public class KuCoinRepository : RepositoryBase, IKuCoinRepository
    {
        private Security security;
        private IRESTRepository _restRepo;
        private string baseUrl;
        private ApiInformation _apiInfo;
        private DateTimeHelper _dtHelper;
        private Helper _helper;
        private bool _systemTimetamp = false;

        /// <summary>
        /// Constructor for non-signed endpoints
        /// </summary>
        /// <param name="sandbox">Use sandbox? (default = false)</param>
        public KuCoinRepository(bool sandbox = false) : base(sandbox)
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
        public KuCoinRepository(string apiKey, string apiSecret, string apiPassword, bool sandbox = false) : this (new ApiInformation { ApiKey = apiKey, ApiSecret = apiSecret, ApiPassword = apiPassword }, sandbox)
        {
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="configPath">String of path to configuration file</param>
        /// <param name="sandbox">Use sandbox? (default = false)</param>
        public KuCoinRepository(string configPath, bool sandbox = false) : base(sandbox)
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
        public KuCoinRepository(ApiInformation apiInformation, bool sandbox = false) : base(apiInformation, sandbox)
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
            security = new Security();
            _restRepo = new RESTRepository();
            baseUrl = "https://openapi-v2.kucoin.com";
            _dtHelper = new DateTimeHelper();
            _helper = new Helper();
            _apiInfo = new ApiInformation
            {
                ApiKey = key,
                ApiSecret = secret,
                ApiPassword = password
            };
            _systemTimetamp = TimestampCompare();
        }

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

        #region Secure Endpoints

        /// <summary>
        /// Get all account balances
        /// </summary>
        /// <param name="hideZeroBalance">Hide zero balance coins</param>
        /// <returns>Balance collection</returns>
        public async Task<List<Balance>> GetBalances(bool hideZeroBalance = false)
        {
            var balances = await OnGetBalances(string.Empty, null);

            return hideZeroBalance
                ? balances.Where(b => b.Total > 0).Distinct().ToList()
                : balances.Distinct().ToList();
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns>Balance collection</returns>
        public async Task<List<Balance>> GetBalances()
        {
            return await OnGetBalances(string.Empty, null);
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Balance collection</returns>
        public async Task<List<Balance>> GetBalances(string symbol)
        {
            return await OnGetBalances(symbol, null);
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="type">Account type</param>
        /// <returns>Balance collection</returns>
        public async Task<List<Balance>> GetBalances(AccountType type)
        {
            return await OnGetBalances(string.Empty, type);
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="type">Account type</param>
        /// <returns>Balance collection</returns>
        public async Task<List<Balance>> GetBalances(string symbol, AccountType type)
        {
            return await OnGetBalances(symbol, type);
        }

        /// <summary>
        /// Get account balances
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="type">Account type</param>
        /// <returns>Balance collection</returns>
        private async Task<List<Balance>> OnGetBalances(string symbol, AccountType? type)
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
        /// <param name="startAt">Start time</param>
        /// <param name="endAt">End time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged response of account history</returns>
        public async Task<PagedResponse<List<AccountAction>>> GetAccountHistory(string accountId, DateTime startAt, DateTime endAt, int page = 0, int pageSize = 0)
        {
            if(startAt >= endAt)
            {
                throw new Exception("Start date cannot be >= End date.");
            }
            var startNonce = _dtHelper.LocalToUnixTime(startAt);
            var endNonce = _dtHelper.LocalToUnixTime(endAt);

            return await GetAccountHistory(accountId, startNonce, endNonce, page, pageSize);
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

            if (startAt >= endAt)
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
        /// <param name="fromId">Account Id Payer</param>
        /// <param name="toId">Account Id Receiver</param>
        /// <param name="amount">Amount to transfer</param>
        /// <returns>Id of funds transfer order</returns>
        public async Task<string> InnerTransfer(string fromId, string toId, decimal amount)
        {
            var clientOid = Guid.NewGuid().ToString().Replace("-","");

            return await InnerTransfer(clientOid, fromId, toId, amount);
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
        /// Place a limit order
        /// </summary>
        /// <param name="parms">Limit Order Parameters</param>
        /// <returns>String of order id</returns>
        public async Task<string> PlaceLimitOrder(LimitOrderParams parms)
        {
            var body = new SortedDictionary<string, object>();
            if (parms.CancelAfter > 0)
                body.Add("cancelAfter", parms.CancelAfter);
            if (string.IsNullOrEmpty(parms.ClientOid))
                parms.ClientOid = Guid.NewGuid().ToString();
            body.Add("clientOid", parms.ClientOid);
            body.Add("symbol", parms.Pair);
            if(parms.PostOnly)
                body.Add("postOnly", parms.PostOnly);
            body.Add("price", parms.Price);
            if(!string.IsNullOrEmpty(parms.Remark))
                body.Add("remark", parms.Remark);
            if(parms.SelfTradeProtect != null)
                body.Add("stp", parms.SelfTradeProtect.ToString());
            body.Add("side", parms.Side.ToString().ToLower());
            body.Add("size", parms.Size);
            if(parms.TimeInForce != null)
                body.Add("timeInForce", parms.TimeInForce);
            body.Add("type", parms.Type.ToString().ToLower());

            return await OnPlaceOrder(body);
        }

        /// <summary>
        /// Place a market order
        /// </summary>
        /// <param name="parms">Market Order Parameters</param>
        /// <returns>String of order id</returns>
        public async Task<string> PlaceMarketOrder(MarketOrderParams parms)
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

            return await OnPlaceOrder(body);
        }

        /// <summary>
        /// Place a stop order
        /// </summary>
        /// <param name="parms">Stop Limit Order Parameters</param>
        /// <returns>String of order id</returns>
        public async Task<string> PlaceStopOrder(StopLimitOrderParams parms)
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
            body.Add("stop", parms.Stop.ToString());
            body.Add("stopPrice", parms.StopPrice);
            if (parms.TimeInForce != null)
                body.Add("timeInForce", parms.TimeInForce.ToString());
            body.Add("type", parms.Type.ToString().ToLower());

            return await OnPlaceOrder(body);
        }

        /// <summary>
        /// Place an order
        /// </summary>
        /// <param name="body">Body of order message</param>
        /// <returns>String of order id</returns>
        private async Task<string> OnPlaceOrder(SortedDictionary<string, object> body)
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

            return await Delete<string>(endpoint);
        }

        /// <summary>
        /// Cancel all open orders
        /// </summary>
        /// <returns>Collection of canceled order Ids</returns>
        public async Task<List<string>> CancelAllOrders()
        {
            var endpoint = $"/api/v1/orders";

            return await Delete<List<string>>(endpoint);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public async Task<PagedResponse<List<Order>>> GetOrders(int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public async Task<PagedResponse<List<Order>>> GetOrders(string pair, int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(pair: pair, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public async Task<PagedResponse<List<Order>>> GetOrders(string pair, Side side, int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(pair: pair, side: side, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="status">Order status</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public async Task<PagedResponse<List<Order>>> GetOrders(string pair, OrderStatus status, int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(pair: pair, status: status, page: page, pageSize: pageSize);
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
        public async Task<PagedResponse<List<Order>>> GetOrders(string pair, Side side, OrderStatus status, int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(pair: pair, side: side, status: status, page: page, pageSize: pageSize);
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
        public async Task<PagedResponse<List<Order>>> GetOrders(string pair, OrderStatus? status, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0)
        {
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await OnGetOrders(status, pair, side, type, startAt, endAt, page, pageSize);
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
        public async Task<PagedResponse<List<Order>>> GetOrders(string pair, OrderStatus? status, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(status, pair, side, type, startAt, endAt, page, pageSize);
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public async Task<PagedResponse<List<Order>>> GetOpenOrders(int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(status: OrderStatus.ACTIVE, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public async Task<PagedResponse<List<Order>>> GetOpenOrders(string pair, int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(status: OrderStatus.ACTIVE, pair: pair, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="side">Trade side</param>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Paged list of Orders</returns>
        public async Task<PagedResponse<List<Order>>> GetOpenOrders(string pair, Side side, int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(OrderStatus.ACTIVE, pair: pair, side: side, page: 0, pageSize: 0);
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
        public async Task<PagedResponse<List<Order>>> GetOpenOrders(string pair, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0)
        {
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await OnGetOrders(OrderStatus.ACTIVE, pair, side, type, startAt, endAt, page, pageSize);
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
        public async Task<PagedResponse<List<Order>>> GetOpenOrders(string pair, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0)
        {
            return await OnGetOrders(OrderStatus.ACTIVE, pair, side, type, startAt, endAt, page, pageSize);
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
        private async Task<PagedResponse<List<Order>>> OnGetOrders(OrderStatus? status = null, string pair = null, Side? side = null, OrderType? type = null, long startAt = 0, long endAt = 0, int page = 1, int pageSize = 50)
        {
            if (startAt >= endAt)
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
        /// Get Fills
        /// </summary>
        /// <returns>Page collection of Fills</returns>
        public async Task<PagedResponse<List<Fill>>> GetFills(int page = 0, int pageSize = 0)
        {
            return await OnGetFills(page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Page collection of Fills</returns>
        public async Task<PagedResponse<List<Fill>>> GetFillsForOrder(string orderId, int page = 0, int pageSize = 0)
        {
            return await OnGetFills(orderId: orderId, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get Fills
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Page collection of Fills</returns>
        public async Task<PagedResponse<List<Fill>>> GetFillsForPair(string pair, int page = 0, int pageSize = 0)
        {
            return await OnGetFills(pair: pair, page: page, pageSize: pageSize);
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
        public async Task<PagedResponse<List<Fill>>> GetFills(string orderId, string pair, Side? side, OrderType? type, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 0)
        {
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await OnGetFills(orderId, pair, side, type, startAt, endAt, page, pageSize);
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
        public async Task<PagedResponse<List<Fill>>> GetFills(string orderId, string pair, Side? side, OrderType? type, long startAt, long endAt, int page = 0, int pageSize = 0)
        {
            return await OnGetFills(orderId, pair, side, type, startAt, endAt, page, pageSize);
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
        private async Task<PagedResponse<List<Fill>>> OnGetFills(string orderId = null, string pair = null, Side? side = null, OrderType? type = null, long startAt = 0, long endAt = 0, int page = 0, int pageSize = 0)
        {
            if (startAt >= endAt)
            {
                throw new Exception("Start time cannot be on or after End time");
            }

            var endpoint = $"/api/v1/orders";

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
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        public async Task<PagedResponse<List<Deposit>>> GetDepositHistory(string symbol, int page = 0, int pageSize = 0)
        {
            return await GetDepositHistory(symbol: symbol, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get deposit history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="status">Deposit status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of Deposits</returns>
        public async Task<PagedResponse<List<Deposit>>> GetDepositHistory(string symbol, DepositStatus status, int page = 0, int pageSize = 0)
        {
            return await GetDepositHistory(symbol: symbol, status: status, page: page, pageSize: pageSize);
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
        public async Task<PagedResponse<List<Deposit>>> GetDepositHistory(string symbol = null, DateTime? startDate = null, DateTime? endDate = null, DepositStatus? status = null, int page = 0, int pageSize = 0)
        {
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await GetDepositHistory(symbol, startAt, endAt, status, page, pageSize);
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
        /// Get withdrawal history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        public async Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(string symbol, int page = 0, int pageSize = 0)
        {
            return await GetWithdrawalHistory(symbol: symbol, page: page, pageSize: pageSize);
        }

        /// <summary>
        /// Get withdrawal history
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <param name="status">Withdrawal status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged collection of withdrawals</returns>
        public async Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(string symbol, DepositStatus status, int page = 0, int pageSize = 0)
        {
            return await GetWithdrawalHistory(symbol: symbol, status: status, page: page, pageSize: pageSize);
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
        public async Task<PagedResponse<List<Withdrawal>>> GetWithdrawalHistory(string symbol = null, DateTime? startDate = null, DateTime? endDate = null, WithdrawalStatus? status = null, int page = 0, int pageSize = 0)
        {
            var startAt = startDate != null ? _dtHelper.LocalToUnixTime((DateTime)startDate) : 0;
            var endAt = endDate != null ? _dtHelper.LocalToUnixTime((DateTime)endDate) : 0;

            return await GetWithdrawalHistory(symbol, startAt, endAt, status, page, pageSize);
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
        public async Task<string> Withdrawal(string symbol, string address, string memo, decimal amount, bool inner, string remark)
        {
            var endpoint = $"/api/v1/withdrawals";

            var url = baseUrl + endpoint;

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
        /// Get current markets on the exchange
        /// </summary>
        /// <param name="trading">Currently trading</param>
        /// <returns>Collection of trading pairs</returns>
        public async Task<List<string>> GetMarkets(bool trading = true)
        {
            var details = await GetTradingPairDetails();

            return details.Select(d => d.Pair).ToList();
        }

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
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        public async Task<List<Candlestick>> GetCandlestick(string pair, Interval interval, int stickCount)
        {
            var endAt = _dtHelper.UTCEndOfMinuteToUnixTime();
            var startAt = _helper.GetFromUnixTime(endAt, interval, (stickCount + 2));

            return await GetCandlestick(pair, startAt, endAt, interval);
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="endAt">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        public async Task<List<Candlestick>> GetCandlestick(string pair, long endAt, Interval interval, int stickCount)
        {
            var startAt = _helper.GetFromUnixTime(endAt, interval, (stickCount + 2));

            return await GetCandlestick(pair, startAt, endAt, interval);
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="endDate">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <param name="stickCount">Number of sticks to return</param>
        /// <returns>Collection of candlesticks</returns>
        public async Task<List<Candlestick>> GetCandlestick(string pair, DateTime endDate, Interval interval, int stickCount)
        {
            var endAt = _dtHelper.LocalToUnixTime(endDate);
            var startAt = _helper.GetFromUnixTime(endAt, interval, (stickCount + 2));

            return await GetCandlestick(pair, startAt, endAt, interval);
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="startDate">Starting date</param>
        /// <param name="endDate">Ending date</param>
        /// <param name="interval">Candlestick interval</param>
        /// <returns>Collection of candlesticks</returns>
        public async Task<List<Candlestick>> GetCandlestick(string pair, DateTime startDate, DateTime endDate, Interval interval)
        {
            var startAt = _dtHelper.LocalToUnixTime(startDate);
            var endAt = _dtHelper.LocalToUnixTime(endDate);
            
            return await GetCandlestick(pair, startAt, endAt, interval);
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
            if(startAt >= endAt)
            {
                throw new Exception("Start time cannot be >= end time.");
            }

            var type = _helper.GetEnumDescription(interval);

            var endpoint = $"/api/v1/market/candles?symbol={pair}&startAt={startAt}&endAt={endAt}&type={type}";

            return await Get<List<Candlestick>>(endpoint, false);
        }

        /// <summary>
        /// Get 24 hour stats for a trading pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>TradingPairStats object</returns>
        public async Task<TradingPairStats> Get24HrStats(string pair)
        {
            var endpoint = $"/api/v1/market/stats?currency={pair}";

            return await Get<TradingPairStats>(endpoint, false);
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
        /// Get server time from KuCoin
        /// </summary>
        /// <returns>Unix server time</returns>
        public async Task<long> GetServerTime()
        {
            var endpoint = "/api/v1/timestamp";

            var response = await base.GetRequest<long>(endpoint);

            return response;
        }

        #endregion Public Endpoints

        #region Helpers

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
            var timestamp = GetTimestamp();

            if (secure)
            {
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