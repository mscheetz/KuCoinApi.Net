using KuCoinApi.Net.Data;
using KuCoinApi.Net.Data.Interface;
using KuCoinApi.Net.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KuCoinApi.Net
{
    public class KuCoinApiClient
    {
        [Obsolete("This initializer is depricated. Use direct method calls instead.")]
        public IKuCoinRepository KuCoinRepository;
        private IKuCoinRepository _repository;

        /// <summary>
        /// Constructor, no authorization
        /// </summary>
        public KuCoinApiClient()
        {
            KuCoinRepository = new KuCoinRepository();
            _repository = new KuCoinRepository();
        }

        /// <summary>
        /// Constructor, with authorization
        /// </summary>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        public KuCoinApiClient(string apiKey, string apiSecret)
        {
            KuCoinRepository = new KuCoinRepository(apiKey, apiSecret);
            _repository = new KuCoinRepository(apiKey, apiSecret);
        }

        /// <summary>
        /// Constructor - with authentication
        /// </summary>
        /// <param name="configPath">Path to config file</param>
        public KuCoinApiClient(string configPath)
        {
            KuCoinRepository = new KuCoinRepository(configPath);
            _repository = new KuCoinRepository(configPath);
        }

        /// <summary>
        /// Check if the Exchange Repository is ready for trading
        /// </summary>
        /// <returns>Boolean of validation</returns>
        public bool ValidateExchangeConfigured()
        {
            return _repository.ValidateExchangeConfigured();
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="pair">String of pair</param>
        /// <param name="size">stick size</param>
        /// <param name="limit">number of sticks</param>
        /// <returns>ChartValue object</returns>
        public ChartValue GetCandlesticks(string pair, Interval size, int limit)
        {
            return _repository.GetCandlesticks(pair, size, limit).Result;
        }

        /// <summary>
        /// Get account balance of a coin
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Balance object</returns>
        public Balance GetBalance(string symbol)
        {
            return _repository.GetBalance(symbol).Result;
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns>Balance array</returns>
        public Balance[] GetBalance()
        {
            return _repository.GetBalances().Result;
        }

        /// <summary>
        /// Get account balances of all coins
        /// </summary>
        /// <returns>Balance array</returns>
        public Balance[] GetBalances()
        {
            return _repository.GetBalances().Result;
        }

        /// <summary>
        /// Get order information
        /// </summary>
        /// <param name="pair">string of pair</param>
        /// <param name="tradeType">Trade type</param>
        /// <param name="orderId">long of orderId</param>
        /// <param name="page">Page number, default 1</param>
        /// <param name="limit">Number of fills to return, default 20</param>
        /// <returns>OrderResponse object</returns>
        public OrderListDetail GetOrder(string pair, TradeType tradeType, long orderId, int page = 1, int limit = 20)
        {
            return _repository.GetOrder(pair, tradeType, orderId, page, limit).Result;
        }

        /// <summary>
        /// Get all current user order information
        /// </summary>
        /// <param name="pair">string of pair</param>
        /// <param name="limit">Int of orders count to return, default 20</param>
        /// <param name="page">Int of page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public OrderListDetail[] GetOrders(string pair, int limit = 20, int page = 1)
        {
            return _repository.GetOrders(pair, limit, page).Result;
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">string of pair</param>
        /// <returns>KuCoinOpenOrders object</returns>
        public OpenOrderResponse<OpenOrder> GetOpenOrders(string pair)
        {
            return _repository.GetOpenOrders(pair).Result;
        }

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <returns>KuCoinOpenOrderDetail objects</returns>
        public OpenOrderResponse<OpenOrderDetail> GetOpenOrderDetails()
        {
            return _repository.GetOpenOrdersDetails().Result;
        }

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <param name="pair">string of trading pair</param>
        /// <returns>KuCoinOpenOrderDetail objects</returns>
        public OpenOrderResponse<OpenOrderDetail> GetOpenOrderDetails(string pair)
        {
            return _repository.GetOpenOrdersDetails(pair).Result;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <returns>OpenOrderResponse object</returns>
        public OrderListDetail[] GetDealtOrders()
        {
            return _repository.GetDealtOrders().Result;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <returns>OpenOrderResponse object</returns>
        public OrderListDetail[] GetDealtOrders(Side side)
        {
            return _repository.GetDealtOrders(side).Result;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>OpenOrderResponse object</returns>
        public OrderListDetail[] GetDealtOrders(string symbol)
        {
            return _repository.GetDealtOrders(symbol).Result;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        public OrderListDetail[] GetDealtOrders(DateTime? from, DateTime? to)
        {
            return _repository.GetDealtOrders(from, to).Result;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        public OrderListDetail[] GetDealtOrders(string symbol = "", Side? side = null, DateTime? from = null, DateTime? to = null)
        {
            return _repository.GetDealtOrders(symbol, side, from, to).Result;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 100</param>
        /// <param name="page">Page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public OrderListDetail[] GetDealtOrders(Side side, int page, int limit)
        {
            return _repository.GetDealtOrders(side, page, limit).Result;
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
        public OrderListDetail[] GetDealtOrders(Side side, int page, int limit, DateTime? from, DateTime? to)
        {
            return _repository.GetDealtOrders(side, page, limit, from, to).Result;
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 20</param>
        /// <param name="page">Page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public OrderListDetail[] GetDealtOrders(string symbol, Side side, int page, int limit)
        {
            return _repository.GetDealtOrders(symbol, side, page, limit).Result;
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
        public OrderListDetail[] GetDealtOrders(string symbol, Side side, int page, int limit, DateTime? from, DateTime? to)
        {
            return _repository.GetDealtOrders(symbol, side, page, limit, from, to).Result;
        }

        /// <summary>
        /// Get Order Book for a pair
        /// </summary>
        /// <param name="pair">string of trading pair</param>
        /// <param name="limit">number of orders to return per side, default 100</param>
        /// <returns>OrderBook object</returns>
        public OrderBookResponse GetOrderBook(string pair, int limit = 100)
        {
            return _repository.GetOrderBook(pair, limit).Result;
        }

        /// <summary>
        /// Place a limit order
        /// </summary>
        /// <param name="pair">String of trading pair</param>
        /// <param name="price">price of trade</param>
        /// <param name="quantity">quantity to trade</param>
        /// <param name="side">trade side</param>
        /// <returns>KuCoinResponse object</returns>
        public ApiResponse<Dictionary<string, string>> LimitOrder(string pair, decimal price, decimal quantity, Side side)
        {
            var parms = new TradeParams
            {
                price = price,
                quantity = quantity,
                symbol = pair,
                side = side.ToString()
            };

            return _repository.PostTrade(parms).Result;
        }

        /// <summary>
        /// Place a market order
        /// </summary>
        /// <param name="pair">String of trading pair</param>
        /// <param name="quantity">quantity to trade</param>
        /// <param name="side">trade side</param>
        /// <returns>KuCoinResponse object</returns>
        public ApiResponse<Dictionary<string, string>> MarketOrder(string pair, decimal quantity, Side side)
        {
            var price = GetTick(pair).lastDealPrice;

            var parms = new TradeParams
            {
                price = price,
                quantity = quantity,
                symbol = pair,
                side = side.ToString()
            };

            return _repository.PostTrade(parms).Result;
        }

        /// <summary>
        /// Post/Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade to place</param>
        /// <returns>KuCoinResponse object</returns>
        public ApiResponse<Dictionary<string, string>> PostTrade(TradeParams tradeParams)
        {
            return _repository.PostTrade(tradeParams).Result;
        }

        /// <summary>
        /// Delete/Cancel a trade
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="orderOid">Order id to cancel</param>
        /// <param name="tradeType">Trade type to cancel</param>
        /// <returns>TradeResponse object</returns>
        public DeleteResponse DeleteTrade(string pair, string orderOid, string tradeType)
        {
            return _repository.DeleteTrade(pair, orderOid, tradeType).Result;
        }

        /// <summary>
        /// Get Ticker for all pairs
        /// </summary>
        /// <returns>Array of KuCoinTick objects</returns>
        public Tick[] GetTicks()
        {
            return _repository.GetTicks().Result;
        }

        /// <summary>
        /// Get Tick for a pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>KuCoinTick object</returns>
        public Tick GetTick(string pair)
        {
            return _repository.GetTick(pair).Result;
        }

        /// <summary>
        /// Get Markets trading on exchange
        /// </summary>
        /// <returns>Array of pair strings</returns>
        public string[] GetMarkets()
        {
            return _repository.GetMarkets().Result;
        }

        /// <summary>
        /// Get details for all coins
        /// </summary>
        /// <returns>Array of Tick objects</returns>
        public Tick[] GetTradingSymbolTick()
        {
            return _repository.GetTradingSymbolTick().Result;
        }

        /// <summary>
        /// Get all trading pairs
        /// </summary>
        /// <returns>Array of trading pair strings</returns>
        public string[] GetTradingPairs()
        {
            return _repository.GetTradingPairs().Result;
        }

        /// <summary>
        /// Get details for a coin
        /// </summary>
        /// <returns>CoinInfo object</returns>
        public CoinInfo GetCoin(string coin)
        {
            return _repository.GetCoin(coin).Result;
        }

        /// <summary>
        /// Get details for all coins
        /// </summary>
        /// <returns>Array of CoinInfo objects</returns>
        public CoinInfo[] GetCoins()
        {
            return _repository.GetCoins().Result;
        }

        /// <summary>
        /// Get open sells
        /// </summary>
        /// <param name="market">Market to check: BTC, ETH, KCS, etc (default = "")</param>
        /// <returns>Array of Trending objects</returns>
        public Trending[] GetTrendings(string market = "")
        {
            return _repository.GetTrendings(market).Result;
        }

        /// <summary>
        /// Get KuCoinTime
        /// </summary>
        /// <returns>long of timestamp</returns>
        public long GetKuCoinTime()
        {
            return _repository.GetKuCoinTime().Result;
        }

        /// <summary>
        /// Get deposit address
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>String of address</returns>
        public string GetDepositAddress(string symbol)
        {
            return _repository.GetDepositAddress(symbol).Result;
        }

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <returns>Boolean of withdraw attempt</returns>
        public bool WithdrawFunds(string symbol, decimal amount, string address)
        {
            return _repository.WithdrawFunds(symbol, amount, address).Result;
        }

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <param name="memo">Address memo</param>
        /// <returns>Boolean of withdraw attempt</returns>
        public bool WithdrawFunds(string symbol, decimal amount, string address, string memo)
        {
            return _repository.WithdrawFunds(symbol, amount, address, memo).Result;
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>Collection of deposits</returns>
        public DealOrder<DepositWithdrawTransaction[]> GetDeposits(string symbol)
        {
            return _repository.GetDeposits(symbol).Result;
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>Collection of withdrawals</returns>
        public DealOrder<DepositWithdrawTransaction[]> GetWithdrawals(string symbol)
        {
            return _repository.GetWithdrawals(symbol).Result;
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <returns>Collection of deposits</returns>
        public DealOrder<DepositWithdrawTransaction[]> GetDeposits(string symbol, DWStatus status)
        {
            return _repository.GetDeposits(symbol, status).Result;
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of withdrawals</param>
        /// <returns>Collection of withdrawals</returns>
        public DealOrder<DepositWithdrawTransaction[]> GetWithdrawals(string symbol, DWStatus status)
        {
            return _repository.GetWithdrawals(symbol, status).Result;
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Collection of deposits</returns>
        public DealOrder<DepositWithdrawTransaction[]> GetDeposits(string symbol, DWStatus status, int page = 1)
        {
            return _repository.GetDeposits(symbol, status, page).Result;
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of withdrawals</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Collection of withdrawals</returns>
        public DealOrder<DepositWithdrawTransaction[]> GetWithdrawals(string symbol, DWStatus status, int page = 1)
        {
            return _repository.GetWithdrawals(symbol, status, page).Result;
        }

        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="size">stick size</param>
        /// <param name="limit">number of sticks</param>
        /// <returns>ChartValue object</returns>
        public async Task<ChartValue> GetCandlesticksAsync(string symbol, Interval size, int limit)
        {
            return await _repository.GetCandlesticks(symbol, size, limit);
        }

        /// <summary>
        /// Get account balance of a coin
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Balance object</returns>
        public async Task<Balance> GetBalanceAsync(string symbol)
        {
            return await _repository.GetBalance(symbol);
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns>Balance array</returns>
        public async Task<Balance[]> GetBalanceAsync()
        {
            return await _repository.GetBalances();
        }

        /// <summary>
        /// Get account balances of all coins
        /// </summary>
        /// <returns>Balance array</returns>
        public async Task<Balance[]> GetBalancesAsync()
        {
            return await _repository.GetBalances();
        }

        /// <summary>
        /// Get order information
        /// </summary>
        /// <param name="pair">string of pair</param>
        /// <param name="tradeType">Trade type</param>
        /// <param name="orderId">long of orderId</param>
        /// <param name="page">Page number, default 1</param>
        /// <param name="limit">Number of fills to return, default 20</param>
        /// <returns>OrderResponse object</returns>
        public async Task<OrderListDetail> GetOrderAsync(string pair, TradeType tradeType, long orderId, int page = 1, int limit = 20)
        {
            return await _repository.GetOrder(pair, tradeType, orderId, page, limit);
        }

        /// <summary>
        /// Get all current user order information
        /// </summary>
        /// <param name="pair">string of pair</param>
        /// <param name="limit">Int of orders count to return, default 20</param>
        /// <param name="page">Int of page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetOrdersAsync(string pair, int limit = 20, int page = 1)
        {
            return await _repository.GetOrders(pair, limit, page);
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrdersAsync()
        {
            return await _repository.GetDealtOrders();
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrdersAsync(Side side)
        {
            return await _repository.GetDealtOrders(side);
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrdersAsync(string symbol)
        {
            return await _repository.GetDealtOrders(symbol);
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrdersAsync(DateTime? from, DateTime? to)
        {
            return await _repository.GetDealtOrders(from, to);
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrdersAsync(string symbol = "", Side? side = null, DateTime? from = null, DateTime? to = null)
        {
            return await _repository.GetDealtOrders(symbol, side, from, to);
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 100</param>
        /// <param name="page">Page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrdersAsync(Side side, int page, int limit)
        {
            return await _repository.GetDealtOrders(side, page, limit);
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
        public async Task<OrderListDetail[]> GetDealtOrdersAsync(Side side, int page, int limit, DateTime? from, DateTime? to)
        {
            return await _repository.GetDealtOrders(side, page, limit, from, to);
        }

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 20</param>
        /// <param name="page">Page number</param>
        /// <returns>OpenOrderResponse object</returns>
        public async Task<OrderListDetail[]> GetDealtOrdersAsync(string symbol, Side side, int page, int limit)
        {
            return await _repository.GetDealtOrders(symbol, side, page, limit);
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
        public async Task<OrderListDetail[]> GetDealtOrdersAsync(string symbol, Side side, int page, int limit, DateTime? from, DateTime? to)
        {
            return await _repository.GetDealtOrders(symbol, side, page, limit, from, to);
        }

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="pair">string of pair</param>
        /// <returns>KuCoinOpenOrders object</returns>
        public async Task<OpenOrderResponse<OpenOrder>> GetOpenOrdersAsync(string pair)
        {
            return await _repository.GetOpenOrders(pair);
        }

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <returns>KuCoinOpenOrderDetail objects</returns>
        public async Task<OpenOrderResponse<OpenOrderDetail>> GetOpenOrdersDetailAsync()
        {
            return await _repository.GetOpenOrdersDetails();
        }

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <param name="pair">string of pair</param>
        /// <returns>KuCoinOpenOrderDetail objects</returns>
        public async Task<OpenOrderResponse<OpenOrderDetail>> GetOpenOrdersDetailAsync(string pair)
        {
            return await _repository.GetOpenOrdersDetails(pair);
        }

        /// <summary>
        /// Get Order Book for a pair
        /// </summary>
        /// <param name="pair">string of trading pair</param>
        /// <param name="limit">number of orders to return per side, default 100</param>
        /// <returns>OrderBook object</returns>
        public async Task<OrderBookResponse> GetOrderBookAsync(string pair, int limit = 100)
        {
            return await _repository.GetOrderBook(pair, limit);
        }

        /// <summary>
        /// Place a limit order
        /// </summary>
        /// <param name="pair">String of trading pair</param>
        /// <param name="price">price of trade</param>
        /// <param name="quantity">quantity to trade</param>
        /// <param name="side">trade side</param>
        /// <returns>KuCoinResponse object</returns>
        public async Task<ApiResponse<Dictionary<string, string>>> LimitOrderAsync(string pair, decimal price, decimal quantity, Side side)
        {
            var parms = new TradeParams
            {
                price = price,
                quantity = quantity,
                symbol = pair,
                side = side.ToString()
            };

            return await _repository.PostTrade(parms);
        }

        /// <summary>
        /// Place a market order
        /// </summary>
        /// <param name="pair">String of trading pair</param>
        /// <param name="quantity">quantity to trade</param>
        /// <param name="side">trade side</param>
        /// <returns>KuCoinResponse object</returns>
        public async Task<ApiResponse<Dictionary<string, string>>> MarketOrderAsync(string pair, decimal quantity, Side side)
        {
            var tick = await GetTickAsync(pair);
            var price = tick.lastDealPrice;
            var parms = new TradeParams
            {
                price = price,
                quantity = quantity,
                symbol = pair,
                side = side.ToString()
            };

            return await _repository.PostTrade(parms);
        }

        /// <summary>
        /// Post/Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade to place</param>
        /// <returns>KuCoinResponse object</returns>
        public async Task<ApiResponse<Dictionary<string, string>>> PostTradeAsync(TradeParams tradeParams)
        {
            return await _repository.PostTrade(tradeParams);
        }

        /// <summary>
        /// Delete/Cancel a trade
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <param name="orderOid">Order id to cancel</param>
        /// <param name="tradeType">Trade type to cancel</param>
        /// <returns>TradeResponse object</returns>
        public async Task<DeleteResponse> DeleteTradeAsync(string pair, string orderOid, string tradeType)
        {
            return await _repository.DeleteTrade(pair, orderOid, tradeType);
        }

        /// <summary>
        /// Get Ticker for all pairs
        /// </summary>
        /// <returns>Array of KuCoinTick objects</returns>
        public async Task<Tick[]> GetTicksAsync()
        {
            return await _repository.GetTicks();
        }

        /// <summary>
        /// Get Tick for a pair
        /// </summary>
        /// <param name="pair">Trading pair</param>
        /// <returns>KuCoinTick object</returns>
        public async Task<Tick> GetTickAsync(string pair)
        {
            return await _repository.GetTick(pair);
        }

        /// <summary>
        /// Get Markets trading on exchange
        /// </summary>
        /// <returns>Array of pair strings</returns>
        public async Task<string[]> GetMarketsAsync()
        {
            return await _repository.GetMarkets();
        }

        /// <summary>
        /// Get details for all coins
        /// </summary>
        /// <returns>Array of Tick objects</returns>
        public async Task<Tick[]> GetTradingSymbolTickAsync()
        {
            return await _repository.GetTradingSymbolTick();
        }

        /// <summary>
        /// Get all trading pairs
        /// </summary>
        /// <returns>Array of trading pair strings</returns>
        public async Task<string[]> GetTradingPairsAsync()
        {
            return await _repository.GetTradingPairs();
        }

        /// <summary>
        /// Get details for a coin
        /// </summary>
        /// <returns>CoinInfo object</returns>
        public async Task<CoinInfo> GetCoinAsync(string coin)
        {
            return await _repository.GetCoin(coin);
        }

        /// <summary>
        /// Get details for all coins
        /// </summary>
        /// <returns>Array of CoinInfo objects</returns>
        public async Task<CoinInfo[]> GetCoinsAsync()
        {
            return await _repository.GetCoins();
        }

        /// <summary>
        /// Get open sells
        /// </summary>
        /// <param name="market">Market to check: BTC, ETH, KCS, etc (default = "")</param>
        /// <returns>Array of Trending objects</returns>
        public async Task<Trending[]> GetTrendingsAsync(string market = "")
        {
            return await _repository.GetTrendings(market);
        }

        /// <summary>
        /// Get deposit address
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>String of address</returns>
        public async Task<string> GetDepositAddressAsync(string symbol)
        {
            return await _repository.GetDepositAddress(symbol);
        }

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <returns>Boolean of withdraw attempt</returns>
        public async Task<bool> WithdrawFundsAsync(string symbol, decimal amount, string address)
        {
            return await _repository.WithdrawFunds(symbol, amount, address);
        }

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <param name="memo">Address memo</param>
        /// <returns>Boolean of withdraw attempt</returns>
        public async Task<bool> WithdrawFundsAsync(string symbol, decimal amount, string address, string memo)
        {
            return await _repository.WithdrawFunds(symbol, amount, address, memo);
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>Collection of deposits</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetDepositsAsync(string symbol)
        {
            return await _repository.GetDeposits(symbol);
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>Collection of withdrawals</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawalsAsync(string symbol)
        {
            return await _repository.GetWithdrawals(symbol);
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <returns>Collection of deposits</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetDepositsAsync(string symbol, DWStatus status)
        {
            return await _repository.GetDeposits(symbol, status);
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <returns>Collection of withdrawals</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawalsAsync(string symbol, DWStatus status)
        {
            return await _repository.GetWithdrawals(symbol, status);
        }

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Collection of deposits</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetDepositsAsync(string symbol, DWStatus status, int page = 1)
        {
            return await _repository.GetWithdrawals(symbol, status, page);
        }

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Collection of withdrawals</returns>
        public async Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawalsAsync(string symbol, DWStatus status, int page = 1)
        {
            return await _repository.GetWithdrawals(symbol, status, page);
        }
    }
}
