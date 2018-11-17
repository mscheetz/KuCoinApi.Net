using KuCoinApi.NetCore.Entities;
using KuCoinApi.NetCore.Core;
using KuCoinApi.NetCore.Data.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using RESTApiAccess.Interface;
using RESTApiAccess;
using DateTimeHelpers;
using FileRepository;

namespace KuCoinApi.NetCore.Data
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
        public KuCoinRepository(string apiKey, string apiSecret)
        {
            LoadRepository(apiKey, apiSecret);
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
        private void LoadRepository(string key = "", string secret = "")
        {
            security = new Security();
            _restRepo = new RESTRepository();
            baseUrl = "https://api.kucoin.com";
            _dtHelper = new DateTimeHelper();
            _helper = new Helper();
            _apiInfo = new ApiInformation
            {
                apiKey = key,
                apiSecret = secret
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
            var ready = string.IsNullOrEmpty(_apiInfo.apiKey) ? false : true;
            if (!ready)
                return false;

            return string.IsNullOrEmpty(_apiInfo.apiSecret) ? false : true;
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="size">stick size</param>
        /// <param name="limit">number of sticks</param>
        /// <returns>ChartValue object</returns>
        public async Task<ChartValue> GetCandlesticks(string symbol, Interval size, int limit)
        {
            var kuPair = _helper.CreateDashedPair(symbol);
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
        /// Get all account balances
        /// </summary>
        /// <param name="hideZeroBalance">Hide zero balance coins</param>
        /// <returns>Balance array</returns>
        public async Task<Balance[]> GetBalances(bool hideZeroBalance = false)
        {
            var currentPage = 0;
            var lastPage = 1;
            var balances = new List<Balance>();

            while (currentPage < lastPage)
            {
                currentPage++;
                var pagedResponse = await OnGetBalances(20, currentPage);

                lastPage = pagedResponse.pageNos;
                
                balances.AddRange(pagedResponse.datas);
            }

            return hideZeroBalance
                ? balances.Where(b => b.balance > 0 || b.freezeBalance > 0).Distinct().ToArray()
                : balances.Distinct().ToArray();
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="limit">Number of balances per page</param>
        /// <param name="pageNo">Page to return</param>
        /// <returns>Balance array</returns>
        public async Task<Balance[]> GetBalances(int limit, int pageNo)
        {
            if (limit <= 0 || pageNo <= 0)
            {
                throw new Exception("limit and pageNo must be greater than 0.");
            }

            var balances = await OnGetBalances(limit, pageNo);

            return balances.datas;
        }

        /// <summary>
        /// Get account balances
        /// </summary>
        /// <param name="limit">Number of balances per page</param>
        /// <param name="pageNo">Page to return</param>
        /// <returns>Balance array</returns>
        private async Task<PagedResponse<Balance[]>> OnGetBalances(int limit, int pageNo)
        {
            var endpoint = "/v1/account/balances";
            var queryString = new List<string>();
            if (limit > 0)
                queryString.Add($"limit={limit}");
            if (pageNo != 0)
                queryString.Add($"page={pageNo}");

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint;
            if(queryString.Count > 0)
                url += $"?{_helper.ArrayToString(queryString.ToArray())}";
            
            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<PagedResponse<Balance[]>>>(url, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        /// <summary>
        /// Get account balance of a coin
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Balance object</returns>
        public async Task<Balance> GetBalance(string symbol)
        {
            var endpoint = $"/v1/account/{symbol}/balance";
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
            var kuPair = _helper.CreateDashedPair(symbol);

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
            var kuPair = _helper.CreateDashedPair(symbol);

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
        /// Get all open orders
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>KuCoinOpenOrders object</returns>
        public async Task<OpenOrderResponse> GetOpenOrders(string symbol)
        {
            var endpoint = "/v1/order/active";
            var kuPair = _helper.CreateDashedPair(symbol);

            var queryString = new List<string>
            {
                $"symbol={kuPair}"
            };

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + $"?{queryString[0]}";

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<OpenOrderResponse>>(url, headers);
            
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
            var kuPair = _helper.CreateDashedPair(symbol);
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
            var kuPair = _helper.CreateDashedPair(tradeParams.symbol);
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
            var kuPair = _helper.CreateDashedPair(symbol);

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
            var kuPair = _helper.CreateDashedPair(symbol);
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
        /// Get GET headers
        /// </summary>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="queryString">Querystring to be passed</param>
        /// <returns>Dictionary of request headers</returns>
        private Dictionary<string, string> GetRequestHeaders(string endpoint, string[] queryString = null)
        {
            var nonce = GetTimestamp().ToString();
            var headers = new Dictionary<string, string>();

            headers.Add("KC-API-KEY", _apiInfo.apiKey);
            headers.Add("KC-API-NONCE", nonce);
            headers.Add("KC-API-SIGNATURE", GetSignature(endpoint, nonce, queryString, 0));
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

            headers.Add("KC-API-KEY", _apiInfo.apiKey);
            headers.Add("KC-API-NONCE", nonce);
            headers.Add("KC-API-SIGNATURE", GetSignature(endpoint, nonce, queryString, postData));
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

            var signature = security.GetKuCoinHMACSignature(_apiInfo.apiSecret, sigString);

            return signature;
        }
    }
}