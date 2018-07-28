using KuCoinApi.NetCore.Entities;
using KuCoinApi.NetCore.Data.Interface;
using System;
using System.Linq;
using Xunit;
using KuCoinApi.NetCore.Data;

namespace KuCoinApi.NetCore.Tests
{
    public class KuCoinRepositoryTests : IDisposable
    {

        private string apiKey = "Enter your key";
        private string apiSecret = "Enter your secret";

        public KuCoinRepositoryTests()
        {
        }

        public void Dispose()
        {
        }

        [Fact]
        public void GetCandlesticksTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);
            var interval = Interval.FifteenM;
            var symbol = "ETH-BTC";

            var sticks = repo.GetCandlesticks(symbol, interval, 10).Result;

            Assert.True(sticks != null);
            Assert.True(sticks.close.Length > 0);
            Assert.True(sticks.open.Length > 0);
        }

        [Fact]
        public void GetAccountBalanceTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);

            var balances = repo.GetBalance().Result;

            Assert.True(balances != null);
        }

        [Fact]
        public void GetOrderBookTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);
            var symbol = "ETH-BTC";

            var orderBook = repo.GetOrderBook(symbol).Result;

            Assert.True(orderBook != null);
            Assert.True(orderBook.buys.Length > 0);
            Assert.True(orderBook.sells.Length > 0);
        }

        [Fact]
        public void GetTicksTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);

            var ticks = repo.GetTicks().Result;

            Assert.True(ticks != null);
        }

        [Fact]
        public void GetTickTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);
            var symbol = "ETH-BTC";

            var tick = repo.GetTick(symbol).Result;

            Assert.True(tick != null);
        }

        [Fact]
        public void GetOrdersTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);
            var symbol = "DCC-BTC";

            var orders = repo.GetOrders(symbol).Result;

            Assert.True(orders != null);
        }

        [Fact]
        public void GetOpenOrdersTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);
            var symbol = "DCC-BTC";

            var orders = repo.GetOpenOrders(symbol).Result;

            Assert.True(orders != null);
        }

        [Fact]
        public void PostTradeTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);
            var symbol = "DCC-BTC";
            var tradeParams = new TradeParams
            {
                price = 0.00000400M,
                quantity = 3000,
                symbol = symbol,
                type = "BUY"
            };

            var orderDetail = repo.PostTrade(tradeParams).Result;

            Assert.True(orderDetail != null);
        }

        [Fact]
        public void GetAndCancelOpenTradeTest()
        {
            IKuCoinRepository repo = new KuCoinRepository(apiKey, apiSecret);
            var symbol = "DCC-BTC";

            var orders = repo.GetOpenOrders(symbol).Result;

            Assert.True(orders != null);

            if (orders.openBuys.Length > 0)
            {
                var orderId = orders.openBuys[0].orderId;

                var cancelDetail = repo.DeleteTrade(symbol, orderId, orders.openBuys[0].type).Result;

                Assert.True(cancelDetail != null);
            }

            if(orders.openSells.Length > 0)
            {
                var orderId = orders.openSells[0].orderId;

                var cancelDetail = repo.DeleteTrade(symbol, orderId, orders.openSells[0].type).Result;

                Assert.True(cancelDetail != null);
            }
        }

        [Fact]
        public void GetMarketsTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();

            var markets = repo.GetMarkets().Result;

            Assert.NotNull(markets);
        }

        [Fact]
        public void GetTradingSymbolTickTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();

            var ticks = repo.GetTradingSymbolTick().Result;

            Assert.NotNull(ticks);
        }

        [Fact]
        public void GetTradingPairsTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();

            var pairs = repo.GetTradingPairs().Result;

            Assert.NotNull(pairs);
        }

        [Fact]
        public void GetCoinTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            var symbol = "KCS";

            var coin = repo.GetCoin(symbol).Result;

            Assert.NotNull(coin);
        }

        [Fact]
        public void GetCoinsTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();

            var coins = repo.GetCoins().Result;

            Assert.NotNull(coins);
        }

        [Fact]
        public void GetTrendingsTest()
        {
            IKuCoinRepository repo = new KuCoinRepository();
            var market = "USDT";

            var trendings = repo.GetTrendings(market).Result;

            Assert.NotNull(trendings);
        }
    }
}
