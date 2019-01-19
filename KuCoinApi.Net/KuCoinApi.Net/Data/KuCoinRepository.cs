using DateTimeHelpers;
using FileRepository;
using KuCoinApi.Net.Core;
using KuCoinApi.Net.Data.Interface;
using KuCoinApi.Net.Entities;
using Newtonsoft.Json;
using RESTApiAccess;
using RESTApiAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace KuCoinApi.Net.Data
{
    public class KuCoinRepository : IKuCoinRepository
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
        public KuCoinRepository()
        {
            LoadRepository();
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        /// <param name="apiPassword">Api password</param>
        public KuCoinRepository(string apiKey, string apiSecret, string apiPassword)
        {
            LoadRepository(apiKey, apiSecret, apiPassword);
        }

        /// <summary>
        /// Constructor for signed endpoints
        /// </summary>
        /// <param name="configPath">String of path to configuration file</param>
        public KuCoinRepository(string configPath)
        {
            IFileRepository _fileRepo = new FileRepository.FileRepository();

            if (_fileRepo.FileExists(configPath))
            {
                _apiInfo = _fileRepo.GetDataFromFile<ApiInformation>(configPath);
                LoadRepository();
            }
            else
            {
                throw new Exception("Config file not found");
            }
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
                return GetKuCoinTime().Result;
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

            return string.IsNullOrEmpty(_apiInfo.ApiSecret) ? false : true;
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

            endpoint = endpoint + $"?{_helper.ObjectToString(parms)}";

            var headers = GetRequestHeaders(endpoint);

            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<List<Balance>>>(url, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
               
        /// <summary>
        /// Get account balance of an account
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <returns>Balance object</returns>
        public async Task<Balance> GetBalance(string accountId)
        {
            var endpoint = $"/v1/accounts/{accountId}";
            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(endpoint, null);

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<Balance>>(url, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

            var url = baseUrl + endpoint;

            var headers = PostRequestHeaders(endpoint, body);

            try
            {
                var response = await _restRepo.PostApi<ApiResponse<string>, SortedDictionary<string, object>>(url, body, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            if (page > 1)
                parms.Add("currentPage", page);
            if (pageSize != 50 && pageSize > 0)
                parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.ObjectToString(parms)}" : string.Empty;

            endpoint += queryString;

            var headers = GetRequestHeaders(endpoint);

            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<PagedResponse<List<AccountAction>>>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            if (page > 1)
                parms.Add("currentPage", page);
            if (pageSize > 0)
                parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"&{_helper.ObjectToString(parms)}" : string.Empty;

            endpoint += queryString;

            var headers = GetRequestHeaders(endpoint);

            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<PagedResponse<List<AccountHold>>>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            var clientOid = Guid.NewGuid().ToString();

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

            var url = baseUrl + endpoint;

            var headers = PostRequestHeaders(endpoint, body);

            try
            {
                var response = await _restRepo.PostApi<ApiResponse<string>, SortedDictionary<string, object>>(url, body, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            body.Add("side", parms.Side.ToString());
            body.Add("size", parms.Size);
            if(parms.TimeInForce != null)
                body.Add("timeInForce", parms.TimeInForce);
            body.Add("type", parms.Type.ToString());

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
            body.Add("side", parms.Side.ToString());
            body.Add("size", parms.Size);
            body.Add("type", parms.Type.ToString());

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
            body.Add("side", parms.Side.ToString());
            body.Add("size", parms.Size);
            body.Add("stop", parms.Stop);
            body.Add("stopPrice", parms.StopPrice);
            if (parms.TimeInForce != null)
                body.Add("timeInForce", parms.TimeInForce);
            body.Add("type", parms.Type.ToString());

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

            var url = baseUrl + endpoint;

            var headers = PostRequestHeaders(endpoint, body);

            try
            {
                var response = await _restRepo.PostApi<string, SortedDictionary<string, object>>(url, body, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="orderId">Id of order to cancel</param>
        /// <returns>Id of order canceled</returns>
        public async Task<string> CancelOrder(string orderId)
        {
            var endpoint = $"/api/v1/orders/{orderId}";

            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(HttpMethod.Delete, endpoint);

            try
            {
                var response = await _restRepo.DeleteApi<string>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Cancel all open orders
        /// </summary>
        /// <returns>Collection of canceled order Ids</returns>
        public async Task<List<string>> CancelAllOrders()
        {
            var endpoint = $"/api/v1/orders";

            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(HttpMethod.Delete, endpoint);

            try
            {
                var response = await _restRepo.DeleteApi<Dictionary<string, List<string>>>(url, headers);

                return response["cancelledOrderIds"];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            if (page > 1)
                parms.Add("currentPage", page);
            if (pageSize != 50)
                parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.ObjectToString(parms)}" : string.Empty;

            endpoint += queryString;

            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(HttpMethod.Get, endpoint);

            try
            {
                var response = await _restRepo.GetApiStream<PagedResponse<List<Order>>>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get an order
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns>An Order</returns>
        public async Task<Order> GetOrder(string orderId)
        {
            var endpoint = $"/api/v1/orders/{orderId}";

            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(HttpMethod.Get, endpoint);

            try
            {
                var response = await _restRepo.GetApiStream<Order>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            if (page > 1)
                parms.Add("currentPage", page);
            if (pageSize != 50)
                parms.Add("pageSize", pageSize);

            var queryString = parms.Count > 0 ? $"?{_helper.ObjectToString(parms)}" : string.Empty;

            endpoint += queryString;

            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(HttpMethod.Get, endpoint);

            try
            {
                var response = await _restRepo.GetApiStream<PagedResponse<List<Fill>>>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(HttpMethod.Post, endpoint, body);

            try
            {
                var response = await _restRepo.PostApi<DepositAddress, SortedDictionary<string, object>>(url, body, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get Deposit address for a currency
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Deposit Address</returns>
        public async Task<DepositAddress> GetDepositAddress(string symbol)
        {
            var endpoint = $"/api/v1/deposit-addresses?currency={symbol}";
            
            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(HttpMethod.Get, endpoint);

            try
            {
                var response = await _restRepo.GetApiStream<DepositAddress>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion Secure Endpoints

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="size">stick size</param>
        /// <param name="limit">number of sticks</param>
        /// <returns>ChartValue object</returns>
        public async Task<ChartValue> GetCandlesticks(string symbol, Interval size, int limit)
        {
            var kuPair = TradingPairValidator(symbol);
            var to = _dtHelper.UTCEndOfMinuteToUnixTime();// _dtHelper.UTCtoUnixTime();
            var from = _helper.GetFromUnixTime(to, size, (limit + 2));
            var kuSize = _helper.IntervalToKuCoinStringInterval(size);
            var endpoint = $"/v1/open/chart/history?symbol={kuPair}&resolution={kuSize}&from={from}&to={to}";
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ChartValue>(url);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="tradeType">Trade type</param>
        /// <param name="orderId">long of orderId</param>
        /// <param name="page">Page number, default 1</param>
        /// <param name="limit">Number of fills to return, default 20</param>
        /// <returns>OrderResponse object</returns>
        public async Task<OrderListDetail> GetOrder(string symbol, TradeType tradeType, long orderId, int page = 1, int limit = 20)
        {
            var endpoint = "/v1/order/detail";
            var url = baseUrl + endpoint;
            var kuPair = TradingPairValidator(symbol);

            var queryString = new List<string>
            {
                $"symbol={kuPair}",
                $"type={tradeType.ToString()}",
                $"limit={limit}",
                $"page={page}",
                $"orderOid={orderId}"
            };

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<DealOrder<OrderListDetail>>>(url, headers);

                return response.data.datas;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get all current user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="limit">Int of orders count to return, default 20</param>
        /// <param name="page">Int of page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetOrders(string symbol, int limit = 20, int page = 1)
        {
            var endpoint = "/v1/deal-orders";
            var kuPair = TradingPairValidator(symbol);

            var queryString = new List<string>
            {
                $"limit={limit}",
                $"page={page}",
                $"symbol={kuPair}"
            };

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + $"?{queryString[0]}&{queryString[1]}&{queryString[2]}";

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<DealOrder<OrderListDetail[]>>>(url, headers);

                return response.data.datas;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders()
        {
            var orders = await OnGetAllDealtOrders(string.Empty, null, null, null);

            return orders;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders(Side side)
        {
            var orders = await OnGetAllDealtOrders(string.Empty, side, null, null);

            return orders;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders(string symbol)
        {
            var orders = await OnGetAllDealtOrders(symbol, null, null, null);

            return orders;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders(DateTime? from, DateTime? to)
        {
            var orders = await OnGetAllDealtOrders(string.Empty, null, from, to);

            return orders;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders(string symbol = "", Side? side = null, DateTime? from = null, DateTime? to = null)
        {
            var orders = await OnGetAllDealtOrders(symbol, side, from, to);

            return orders;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 100</param>
        /// <param name="page">Page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders(Side side, int page, int limit)
        {
            limit = limit > 100 ? 100 : limit;
            var response = await OnGetDealtOrders(string.Empty, side, limit, page, null, null);

            return response.datas;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 100</param>
        /// <param name="page">Page number</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders(Side side, int page, int limit, DateTime? from, DateTime? to)
        {
            limit = limit > 100 ? 100 : limit;
            var response = await OnGetDealtOrders(string.Empty, side, limit, page, from, to);

            return response.datas;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 20</param>
        /// <param name="page">Page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders(string symbol, Side side, int page, int limit)
        {
            limit = limit > 20 ? 20 : limit;
            var response = await OnGetDealtOrders(symbol, side, limit, page, null, null);

            return response.datas;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 20</param>
        /// <param name="page">Page number</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrders(string symbol, Side side, int page, int limit, DateTime? from, DateTime? to)
        {
            limit = limit > 20 ? 20 : limit;
            var response = await OnGetDealtOrders(symbol, side, limit, page, from, to);

            return response.datas;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        private async Task<OrderListDetail[]> OnGetAllDealtOrders(string symbol, Side? side, DateTime? from, DateTime? to)
        {
            var limit = string.IsNullOrEmpty(symbol) ? 100 : 20;
            var orderList = new List<OrderListDetail>();

            var currentPage = 0;
            var maxPage = 1;
            if(side == null || side == Side.BUY)
            {
                while (currentPage < maxPage)
                {
                    currentPage++;
                    var response = await OnGetDealtOrders(symbol, Side.BUY, limit, currentPage, from, to);

                    orderList.AddRange(response.datas);
                    maxPage = response.pageNos;
                }
            }
            if (side == null || side == Side.SELL)
            {
                while (currentPage < maxPage)
                {
                    currentPage++;
                    var response = await OnGetDealtOrders(symbol, Side.SELL, limit, currentPage, from, to);
                    
                    orderList.AddRange(response.datas);
                    maxPage = response.pageNos;
                }
            }

            return orderList.OrderByDescending(o => o.createdAt).ToArray();
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Int of orders count to return</param>
        /// <param name="page">Page number</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        private async Task<DealOrder<OrderListDetail[]>> OnGetDealtOrders(string symbol, Side side, int limit, int page, DateTime? from, DateTime? to)
        {
            var endpoint = "/v1/order/dealt";

            var fromTS = from != null ? _dtHelper.LocalToUnixTime((DateTime)from) : 0;
            var toTS = to != null ? _dtHelper.LocalToUnixTime((DateTime)to) : 0;
            var queryString = new List<string>();

            if (to != null)
                queryString.Add($"before={toTS}");
            queryString.Add($"limit={limit}");
            if (page > 1)
                queryString.Add($"page={page}");
            if (from != null)
                queryString.Add($"since={fromTS}");
            if (!string.IsNullOrEmpty(symbol))
            {
                var kuPair = TradingPairValidator(symbol);
                queryString.Add($"symbol={kuPair}");
            }
            queryString.Add($"type={side.ToString()}");

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + $"?{_helper.ArrayToString(queryString.ToArray())}";

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<DealOrder<OrderListDetail[]>>>(url, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>KuCoinOpenOrders object</returns>
        public async Task<OpenOrderResponse<OpenOrder>> GetOpenOrders(string symbol)
        {
            var endpoint = "/v1/order/active";
            var kuPair = TradingPairValidator(symbol);

            var queryString = new List<string>
            {
                $"symbol={kuPair}"
            };

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + $"?{_helper.ListToString(queryString)}";

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<OpenOrderResponse<OpenOrder>>>(url, headers);
            
                return response.data;            
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <returns>KuCoinOpenOrdersResponse object</returns>
        public async Task<OpenOrderResponse<OpenOrderDetail>> GetOpenOrdersDetails()
        {
            return await OnGetOpenOrdersDetails(null, null);
        }

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>KuCoinOpenOrdersResponse object</returns>
        public async Task<OpenOrderResponse<OpenOrderDetail>> GetOpenOrdersDetails(string symbol)
        {
            return await OnGetOpenOrdersDetails(symbol, null);
        }

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="type">Type of trade</param>
        /// <returns>KuCoinOpenOrdersResponse object</returns>
        public async Task<OpenOrderResponse<OpenOrderDetail>> GetOpenOrdersDetails(string symbol, Side type)
        {
            return await OnGetOpenOrdersDetails(symbol, type.ToString());
        }

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="type">Type of trade</param>
        /// <returns>KuCoinOpenOrdersResponse object</returns>
        private async Task<OpenOrderResponse<OpenOrderDetail>> OnGetOpenOrdersDetails(string symbol, string type)
        {
            var endpoint = "/v1/order/active-map";
            var kuPair = !string.IsNullOrEmpty(symbol) ? TradingPairValidator(symbol) : string.Empty;

            var queryString = new List<string>
            {
                $"symbol={kuPair}"
            };

            if(!string.IsNullOrEmpty(type))
            {
                queryString.Add($"type={type}");
            }

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + $"?{_helper.ListToString(queryString)}";

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<OpenOrderResponse<OpenOrderDetail>>>(url, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get Order Book for a pair
        /// </summary>
        /// <param name="symbol">string of trading pair</param>
        /// <param name="limit">number of orders to return per side, default 100</param>
        /// <returns>OrderBook object</returns>
        public async Task<OrderBookResponse> GetOrderBook(string symbol, int limit = 100)
        {
            var kuPair = TradingPairValidator(symbol);
            var endpoint = $"/v1/open/orders?symbol={kuPair}&limit={limit}";
            var url = baseUrl + endpoint;
            
            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<OrderBookResponse>>(url);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Post/Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade to place</param>
        /// <returns>KuCoinResponse object</returns>
        public async Task<ApiResponse<Dictionary<string, string>>> PostTrade(TradeParams tradeParams)
        {
            var kuPair = TradingPairValidator(tradeParams.symbol);
            var endpoint = $"/v1/order";
            var sigEndpoint = $"/v1/{kuPair}/order";
            var queryString = new List<string>
            {
                $"amount={_helper.DecimalToString(tradeParams.quantity)}",
                $"price={_helper.DecimalToString(tradeParams.price)}",
                $"symbol={kuPair}",
                $"type={tradeParams.side}"
            };

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + $"?{_helper.ArrayToString(queryString.ToArray())}";

            try
            {
                var response = await _restRepo.PostApi<ApiResponse<Dictionary<string, string>>, List<string>>(url, queryString, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Delete/Cancel a trade
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <param name="orderOid">Order id to cancel</param>
        /// <param name="tradeType">Trade type to cancel</param>
        /// <returns>TradeResponse object</returns>
        public async Task<DeleteResponse> DeleteTrade(string symbol, string orderOid, string tradeType)
        {
            var endpoint = "/v1/cancel-order";
            if(symbol.IndexOf("-")<0)
            { }
            var kuPair = TradingPairValidator(symbol);

            var queryString = new List<string>
            {
                $"symbol={kuPair}",
                $"orderOid={orderOid}",
                $"type={tradeType}"
            };

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + $"?{_helper.ArrayToString(queryString.ToArray())}";

            try
            {
                var response = await _restRepo.PostApi<DeleteResponse>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get Ticker for all pairs
        /// </summary>
        /// <returns>Array of KuCoinTick objects</returns>
        public async Task<Tick[]> GetTicks()
        {
            var endpoint = "/v1/open/tick";
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<Tick[]>>(url);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get Tick for a symbol
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <returns>KuCoinTick object</returns>
        public async Task<Tick> GetTick(string symbol)
        {
            var kuPair = TradingPairValidator(symbol);
            var endpoint = $"/v1/open/tick?symbol={kuPair}";
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<Tick>>(url);

                return response.data;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get Markets trading on exchange
        /// </summary>
        /// <returns>Array of symbol strings</returns>
        public async Task<string[]> GetMarkets()
        {
            var endpoint = $"/v1/open/markets";
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<string[]>>(url);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get details for all coins
        /// </summary>
        /// <returns>Array of Tick objects</returns>
        public async Task<Tick[]> GetTradingSymbolTick()
        {
            var endpoint = $"/v1/market/open/symbols";
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<Tick[]>>(url);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get all trading pairs
        /// </summary>
        /// <returns>Array of trading pair strings</returns>
        public async Task<string[]> GetTradingPairs()
        {
            var endpoint = $"/v1/market/open/symbols";
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<Tick[]>>(url);

                var pairs = new List<string>();
                for(var i = 0; i < response.data.Length; i++)
                {
                    var pair = string.Empty;
                    pair = response.data[i].coinType + "-" + response.data[i].coinTypePair;
                    pairs.Add(pair);
                }

                return pairs.OrderBy(p => p).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get details for a coin
        /// </summary>
        /// <returns>CoinInfo object</returns>
        public async Task<CoinInfo> GetCoin(string coin)
        {
            var endpoint = $"/v1/market/open/coin-info?coin={coin}";
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<CoinInfo>>(url);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get details for all coins
        /// </summary>
        /// <returns>Array of CoinInfo objects</returns>
        public async Task<CoinInfo[]> GetCoins()
        {
            var endpoint = $"/v1/market/open/coins";
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<CoinInfo[]>>(url);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get open sells
        /// </summary>
        /// <param name="market">Market to check: BTC, ETH, KCS, etc (default = "")</param>
        /// <returns>Array of Trending objects</returns>
        public async Task<Trending[]> GetTrendings(string market = "")
        {
            var endpoint = $"/v1/market/open/coins-trending";
            if (!string.IsNullOrEmpty(market))
            {
                endpoint += $"?market={market}";
            }
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<Trending[]>>(url);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get KuCoinTime
        /// </summary>
        /// <returns>long of timestamp</returns>
        public async Task<long> GetKuCoinTime()
        {
            var endpoint = "/v1/time";
            var url = baseUrl + endpoint;

            var response = await _restRepo.GetApi<Dictionary<string, object>>(url);

            return Convert.ToInt64(response["timestamp"]);
        }

        /// <summary>
        /// Get deposit address
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>String of address</returns>
        public async Task<string> GetDepositAddress(string symbol)
        {
            var endpoint = $"/v1/account/{symbol}/wallet/address";

            var headers = GetRequestHeaders(endpoint);

            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApi<ApiResponse<Dictionary<string, object>>>(url, headers);

                return response.data["address"].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <returns>Boolean of withdraw attempt</returns>
        public async Task<bool> WithdrawFunds(string symbol, decimal amount, string address)
        {
            return await OnWithdrawFunds(symbol, amount, address, string.Empty);
        }

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <param name="memo">Address memo</param>
        /// <returns>Boolean of withdraw attempt</returns>
        public async Task<bool> WithdrawFunds(string symbol, decimal amount, string address, string memo)
        {
            return await OnWithdrawFunds(symbol, amount, address, memo);
        }

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <param name="memo">Address memo</param>
        /// <returns>Boolean of withdraw attempt</returns>
        private async Task<bool> OnWithdrawFunds(string symbol, decimal amount, string address, string memo)
        {
            var endpoint = $"/v1/account/{symbol}/withdraw/apply";

            address = !string.IsNullOrEmpty(memo) ? $"{address}@{memo}" : address;

            var queryString = new List<string>
            {
                $"address={address}",
                $"amount={_helper.DecimalToString(amount)}",
                $"coin={symbol}"
            };

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + "?" + _helper.ArrayToString(queryString.ToArray());

            try
            {
                var response = await _restRepo.PostApi<ApiResponse<string>>(url, headers);

                return response.success;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>Collection of deposits</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetDeposits(string symbol)
        {
            return await OnGetDepositsAndWithdrawals(symbol, DWType.DEPOSIT, DWStatus.NONE, 1);
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>Collection of withdrawals</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawals(string symbol)
        {
            return await OnGetDepositsAndWithdrawals(symbol, DWType.WITHDRAW, DWStatus.NONE, 1);
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <returns>Collection of deposits</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetDeposits(string symbol, DWStatus status)
        {
            return await OnGetDepositsAndWithdrawals(symbol, DWType.DEPOSIT, status, 1);
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of withdrawals</param>
        /// <returns>Collection of withdrawals</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawals(string symbol, DWStatus status)
        {
            return await OnGetDepositsAndWithdrawals(symbol, DWType.WITHDRAW, status, 1);
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Collection of deposits</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetDeposits(string symbol, DWStatus status, int page = 1)
        {
            return await OnGetDepositsAndWithdrawals(symbol, DWType.DEPOSIT, status, page);
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of withdrawals</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Collection of withdrawals</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawals(string symbol, DWStatus status, int page = 1)
        {
            return await OnGetDepositsAndWithdrawals(symbol, DWType.WITHDRAW, status, page);
        }

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="type">Type to return</param>
        /// <param name="status">Status of action</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Boolean of withdraw attempt</returns>
        private async Task<DealOrder<DepositWithdrawTransaction[]>> OnGetDepositsAndWithdrawals(string symbol, DWType type, DWStatus status, int page = 1)
        {
            var endpoint = $"/v1/account/{symbol}/wallet/records";

            var queryString = new List<string>();

            queryString.Add($"page={page}");
            if (status != DWStatus.NONE)
            {
                queryString.Add($"status={status.ToString()}");
            }
            queryString.Add($"type={type.ToString()}");

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + "?" + _helper.ArrayToString(queryString.ToArray());

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<DealOrder<DepositWithdrawTransaction[]>>>(url, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Validate a trading pair
        /// </summary>
        /// <param name="pair">Pair to validate</param>
        /// <returns>Validated trading pair</returns>
        private string TradingPairValidator(string pair)
        {
            if(string.IsNullOrEmpty(pair))
            {
                throw new Exception("Trading pair required.");
            }
            if (pair.IndexOf("-") < 0)
            {
                var markets = this.GetMarkets().Result;
                pair = _helper.CreateDashedPair(pair, markets);
            }

            return pair;
        }

        /// <summary>
        /// Get Request headers
        /// </summary>
        /// <param name="httpMethod">Http Method</param>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="body">Body data to be passed</param>
        /// <returns>Dictionary of request headers</returns>
        private Dictionary<string, string> GetRequestHeaders(HttpMethod httpMethod, string endpoint, SortedDictionary<string, object> body = null)
        {
            var nonce = GetTimestamp().ToString();
            var headers = new Dictionary<string, string>();

            headers.Add("KC-API-KEY", _apiInfo.ApiKey);
            headers.Add("KC-API-SIGN", GetSignature(httpMethod, endpoint, nonce, body));
            headers.Add("KC-API-TIMESTAMP", nonce);
            headers.Add("KC-API-PASSPHRASE", _apiInfo.ApiPassword);
            headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)");

            return headers;
        }

        /// <summary>
        /// Get GET headers
        /// </summary>
        /// <param name="endpoint">Endpoint to access</param>
        /// <returns>Dictionary of request headers</returns>
        private Dictionary<string, string> GetRequestHeaders(string endpoint)
        {
            var nonce = GetTimestamp().ToString();
            var headers = new Dictionary<string, string>();

            headers.Add("KC-API-KEY", _apiInfo.ApiKey);
            headers.Add("KC-API-SIGN", GetSignature(HttpMethod.Get, endpoint, nonce, null));
            headers.Add("KC-API-TIMESTAMP", nonce);
            headers.Add("KC-API-PASSPHRASE", _apiInfo.ApiPassword);
            headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)");

            return headers;
        }

        /// <summary>
        /// Get POST headers
        /// </summary>
        /// <typeparam name="T">Type of data in post request</typeparam>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="body">Body data to be passed</param>
        /// <returns>Dictionary of request headers</returns>
        private Dictionary<string, string> PostRequestHeaders(string endpoint, SortedDictionary<string, object> body)
        {
            var nonce = GetTimestamp().ToString();
            var headers = new Dictionary<string, string>();

            headers.Add("KC-API-KEY", _apiInfo.ApiKey);
            headers.Add("KC-API-SIGN", GetSignature(HttpMethod.Post, endpoint, nonce, body));
            headers.Add("KC-API-TIMESTAMP", nonce);
            headers.Add("KC-API-PASSPHRASE", _apiInfo.ApiPassword);
            headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)");

            return headers;
        }

        /// <summary>
        /// Get GET headers
        /// </summary>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="queryString">Querystring to be passed</param>
        /// <returns>Dictionary of request headers</returns>
        private Dictionary<string, string> GetRequestHeaders(string endpoint, string[] queryString = null)
        {
            var nonce = GetTimestamp().ToString();
            var headers = new Dictionary<string, string>();

            headers.Add("KC-API-KEY", _apiInfo.ApiKey);
            headers.Add("KC-API-SIGN", GetSignature(endpoint, nonce, queryString, 0));
            headers.Add("KC-API-TIMESTAMP", nonce);
            headers.Add("KC-API-PASSPHRASE", _apiInfo.ApiPassword);
            headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)");

            return headers;
        }

        /// <summary>
        /// Get POST headers
        /// </summary>
        /// <typeparam name="T">Type of data in post request</typeparam>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="queryString">Querystring to be passed</param>
        /// <param name="postData">Data to be sent</param>
        /// <returns>Dictionary of request headers</returns>
        private Dictionary<string, string> PostRequestHeaders<T>(string endpoint, string[] queryString, T postData)
        {
            var nonce = GetTimestamp().ToString();
            var headers = new Dictionary<string, string>();

            headers.Add("KC-API-KEY", _apiInfo.ApiKey);
            headers.Add("KC-API-SIGN", GetSignature(endpoint, nonce, queryString, postData));
            headers.Add("KC-API-TIMESTAMP", nonce);
            headers.Add("KC-API-PASSPHRASE", _apiInfo.ApiPassword);
            headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)");

            return headers;
        }

        /// <summary>
        /// Create signature for message
        /// </summary>
        /// <typeparam name="T">Type of data in post request</typeparam>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="nonce">Current nonce</param>
        /// <param name="queryString">Querystring to be passed</param>
        /// <param name="value">Data to be sent</param>
        /// <returns>String of signature</returns>
        private string GetSignature(HttpMethod method, string endpoint, string nonce, SortedDictionary<string, object> parms = null)
        {
            var callMethod = method.ToString().ToUpper();

            var jsonedParams = parms.Count > 0 
                ? JsonConvert.SerializeObject(parms) 
                : string.Empty;

            var sigString = $"{nonce}{callMethod}{endpoint}{jsonedParams}";

            var signature = security.GetKuCoinHMACSignature(_apiInfo.ApiSecret, sigString);

            return signature;
        }

        /// <summary>
        /// Create signature for message
        /// </summary>
        /// <typeparam name="T">Type of data in post request</typeparam>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="nonce">Current nonce</param>
        /// <param name="queryString">Querystring to be passed</param>
        /// <param name="value">Data to be sent</param>
        /// <returns>String of signature</returns>
        private string GetSignature<T>(string endpoint, string nonce, string[] queryString = null, T value = default(T))
        {
            queryString = queryString ?? new string[0];

            Array.Sort(queryString, StringComparer.InvariantCulture);

            var qsValues = string.Empty;

            if(queryString.Length> 0)
            {
                qsValues = _helper.ArrayToString(queryString);
            }
            else if(typeof(T) != typeof(int))
            {
                qsValues += _helper.ObjectToString<T>(value);
            }

            var sigString = $"{endpoint}/{nonce}/{qsValues}";

            var signature = security.GetKuCoinHMACSignature(_apiInfo.ApiSecret, sigString);

            return signature;
        }
    }
}