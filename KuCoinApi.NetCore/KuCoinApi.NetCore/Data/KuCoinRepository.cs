using KuCoinApi.NetCore.Entities;
using KuCoinApi.NetCore.Core;
using KuCoinApi.NetCore.Data.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
//using RESTApiAccess.Interface;
//using RESTApiAccess;
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
                return null;
            }
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns>Balance array</returns>
        public async Task<Balance[]> GetBalance()
        {
            var endpoint = "/v1/account/balance";
            var url = baseUrl + endpoint;

            var headers = GetRequestHeaders(endpoint, null);

            try
            {
                var response = await _restRepo.GetApiStream<ApiResponse<Balance[]>>(url, headers);

                return response.data;
            }
            catch (Exception ex)
            {
                return null;
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
                return null;
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
                return null;
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
                return null;
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
                return null;
            }
        }

        /// <summary>
        /// Post/Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade to place</param>
        /// <returns>KuCoinResponse object</returns>
        public async Task<ApiResponse<Dictionary<string, string>>> PostTrade(TradeParams tradeParams)
        {
            var endpoint = $"/v1/order";
            var kuPair = _helper.CreateDashedPair(tradeParams.symbol);

            var queryString = new List<string>
            {
                $"symbol={kuPair}",
                $"amount={tradeParams.quantity}",
                $"price={tradeParams.price}",
                $"type={tradeParams.side}"
            };

            var headers = GetRequestHeaders(endpoint, queryString.ToArray());

            var url = baseUrl + endpoint + $"?{_helper.ArrayToString(queryString.ToArray())}";

            try
            {
                var response = await _restRepo.PostApi<ApiResponse<Dictionary<string, string>>>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                return null;
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
                return null;
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
                return null;
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
                return null;
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
                return null;
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
                return null;
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
                return null;
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
                return null;
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
                return null;
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
                return null;
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

            var response = await _restRepo.GetApi<long>(url);

            return response;
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
                return null;
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
            var nonce = _dtHelper.UTCtoUnixTimeMilliseconds().ToString();
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
            var nonce = _dtHelper.UTCtoUnixTimeMilliseconds().ToString();
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