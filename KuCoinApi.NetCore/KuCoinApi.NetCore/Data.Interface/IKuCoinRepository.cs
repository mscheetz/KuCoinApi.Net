using KuCoinApi.NetCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KuCoinApi.NetCore.Data.Interface
{
    public interface IKuCoinRepository
    {
        /// <summary>
        /// Check if the Exchange Repository is ready for trading
        /// </summary>
        /// <returns>Boolean of validation</returns>
        bool ValidateExchangeConfigured();
        
        /// <summary>
        /// Get candlesticks
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="size">stick size</param>
        /// <param name="limit">number of sticks</param>
        /// <returns>ChartValue object</returns>
        Task<ChartValue> GetCandlesticks(string symbol, Interval size, int limit);

        /// <summary>
        /// Get all account balances
        /// </summary>
        /// <param name="hideZeroBalance">Hide zero balance coins</param>
        /// <returns>Balance array</returns>
        Task<Balance[]> GetBalances(bool hideZeroBalance = false);

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <param name="limit">Number of balances per page</param>
        /// <param name="pageNo">Page to return</param>
        /// <returns>Balance array</returns>
        Task<Balance[]> GetBalances(int limit, int pageNo);

        /// <summary>
        /// Get account balance of a coin
        /// </summary>
        /// <param name="symbol">Symbol of currency</param>
        /// <returns>Balance object</returns>
        Task<Balance> GetBalance(string symbol);

        /// <summary>
        /// Get order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="tradeType">Trade type</param>
        /// <param name="orderId">long of orderId</param>
        /// <param name="page">Page number, default 1</param>
        /// <param name="limit">Number of fills to return, default 20</param>
        /// <returns>OrderResponse object</returns>
        Task<OrderListDetail> GetOrder(string symbol, TradeType tradeType, long orderId, int page = 1, int limit = 20);

        /// <summary>
        /// Get all current user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="limit">Int of orders count to return, default 20</param>
        /// <param name="page">Int of page number</param>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetOrders(string symbol, int limit = 20, int page = 1);

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetDealtOrders();

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetDealtOrders(Side side);

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetDealtOrders(string symbol);

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetDealtOrders(DateTime? from, DateTime? to);

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetDealtOrders(string symbol = "", Side? side = null, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 100</param>
        /// <param name="page">Page number</param>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetDealtOrders(Side side, int page, int limit);

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 100</param>
        /// <param name="page">Page number</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetDealtOrders(Side side, int page, int limit, DateTime? from, DateTime? to);

        /// <summary>
        /// Get all user order information
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="side">Trade side</param>
        /// <param name="limit">Orders to return, max 20</param>
        /// <param name="page">Page number</param>
        /// <returns>OpenOrderResponse object</returns>
        Task<OrderListDetail[]> GetDealtOrders(string symbol, Side side, int page, int limit);

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
        Task<OrderListDetail[]> GetDealtOrders(string symbol, Side side, int page, int limit, DateTime? from, DateTime? to);

        /// <summary>
        /// Get all open orders
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>KuCoinOpenOrders object</returns>
        Task<OpenOrderResponse<OpenOrder>> GetOpenOrders(string symbol);

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <returns>KuCoinOpenOrdersResponse object</returns>
        Task<OpenOrderResponse<OpenOrderDetail>> GetOpenOrdersDetails();

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <returns>KuCoinOpenOrdersResponse object</returns>
        Task<OpenOrderResponse<OpenOrderDetail>> GetOpenOrdersDetails(string symbol);

        /// <summary>
        /// Get all open orders with details
        /// </summary>
        /// <param name="symbol">string of symbol</param>
        /// <param name="type">Type of trade</param>
        /// <returns>KuCoinOpenOrdersResponse object</returns>
        Task<OpenOrderResponse<OpenOrderDetail>> GetOpenOrdersDetails(string symbol, Side type);

        /// <summary>
        /// Get Order Book for a pair
        /// </summary>
        /// <param name="symbol">string of trading pair</param>
        /// <param name="limit">number of orders to return per side, default 100</param>
        /// <returns>OrderBook object</returns>
        Task<OrderBookResponse> GetOrderBook(string symbol, int limit = 100);

        /// <summary>
        /// Post/Place a trade
        /// </summary>
        /// <param name="tradeParams">Trade to place</param>
        /// <returns>KuCoinResponse object</returns>
        Task<ApiResponse<Dictionary<string, string>>> PostTrade(TradeParams tradeParams);

        /// <summary>
        /// Delete/Cancel a trade
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <param name="orderOid">Order id to cancel</param>
        /// <param name="tradeType">Trade type to cancel</param>
        /// <returns>TradeResponse object</returns>
        Task<DeleteResponse> DeleteTrade(string symbol, string orderOid, string tradeType);

        /// <summary>
        /// Get Ticker for all pairs
        /// </summary>
        /// <returns>Array of KuCoinTick objects</returns>
        Task<Tick[]> GetTicks();

        /// <summary>
        /// Get Tick for a symbol
        /// </summary>
        /// <param name="symbol">Trading symbol</param>
        /// <returns>KuCoinTick object</returns>
        Task<Tick> GetTick(string symbol);

        /// <summary>
        /// Get Markets trading on exchange
        /// </summary>
        /// <returns>Array of symbol strings</returns>
        Task<string[]> GetMarkets();

        /// <summary>
        /// Get details for all coins
        /// </summary>
        /// <returns>Array of Tick objects</returns>
        Task<Tick[]> GetTradingSymbolTick();

        /// <summary>
        /// Get all trading pairs
        /// </summary>
        /// <returns>Array of trading pair strings</returns>
        Task<string[]> GetTradingPairs();

        /// <summary>
        /// Get details for a coin
        /// </summary>
        /// <returns>CoinInfo object</returns>
        Task<CoinInfo> GetCoin(string coin);

        /// <summary>
        /// Get details for all coins
        /// </summary>
        /// <returns>Array of CoinInfo objects</returns>
        Task<CoinInfo[]> GetCoins();

        /// <summary>
        /// Get open sells
        /// </summary>
        /// <param name="market">Market to check: BTC, ETH, KCS, etc (default = "")</param>
        /// <returns>Array of Trending objects</returns>
        Task<Trending[]> GetTrendings(string market = "");

        /// <summary>
        /// Get KuCoinTime
        /// </summary>
        /// <returns>long of timestamp</returns>
        Task<long> GetKuCoinTime();

        /// <summary>
        /// Get deposit address
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>String of address</returns>
        Task<string> GetDepositAddress(string symbol);

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <returns>Boolean of withdraw attempt</returns>
        Task<bool> WithdrawFunds(string symbol, decimal amount, string address);

        /// <summary>
        /// Withdraw funds from exchange
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="amount">Amount to send</param>
        /// <param name="address">Address to send funds</param>
        /// <param name="memo">Address memo</param>
        /// <returns>Boolean of withdraw attempt</returns>
        Task<bool> WithdrawFunds(string symbol, decimal amount, string address, string memo);

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>Collection of deposits</returns>
        Task<DealOrder<DepositWithdrawTransaction[]>> GetDeposits(string symbol);

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <returns>Collection of withdrawals</returns>
        Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawals(string symbol);

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <returns>Collection of deposits</returns>
        Task<DealOrder<DepositWithdrawTransaction[]>> GetDeposits(string symbol, DWStatus status);

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of withdrawals</param>
        /// <returns>Collection of withdrawals</returns>
        Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawals(string symbol, DWStatus status);

        /// <summary>
        /// List account deposits
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of deposit</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Collection of deposits</returns>
        Task<DealOrder<DepositWithdrawTransaction[]>> GetDeposits(string symbol, DWStatus status, int page = 1);

        /// <summary>
        /// List account withdrawals
        /// </summary>
        /// <param name="symbol">String of symbol</param>
        /// <param name="status">Status of withdrawals</param>
        /// <param name="page">Page to return (default = 1)</param>
        /// <returns>Collection of withdrawals</returns>
        Task<DealOrder<DepositWithdrawTransaction[]>> GetWithdrawals(string symbol, DWStatus status, int page = 1);

        /// <summary>
        /// Get WSS token
        /// </summary>
        /// <returns>String of token</returns>
        Task<string> GetWSSToken();
    }
}
