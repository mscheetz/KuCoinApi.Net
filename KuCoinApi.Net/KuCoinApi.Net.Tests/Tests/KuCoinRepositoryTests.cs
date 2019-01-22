using KuCoinApi.Net.Entities;
using KuCoinApi.Net.Data.Interface;
using System;
using System.Linq;
using Xunit;
using KuCoinApi.Net.Data;
using FileRepository;

namespace KuCoinApi.Net.Tests
{
    public class KuCoinRepositoryTests : IDisposable
    {
        private ApiInformation _exchangeApi = null;
        private IKuCoinDotNet _service;
        private string configPath = "config.json";
        private string apiKey = string.Empty;
        private string apiSecret = string.Empty;

        public KuCoinRepositoryTests()
        {
            var useSandbox = true;
            IFileRepository _fileRepo = new FileRepository.FileRepository();
            if (_fileRepo.FileExists(configPath))
            {
                _exchangeApi = _fileRepo.GetDataFromFile<ApiInformation>(configPath);
            }
            if (_exchangeApi != null || !string.IsNullOrEmpty(apiKey))
            {
                _service = new KuCoinDotNet(_exchangeApi, useSandbox);
            }
            else
            {
                _service = new KuCoinDotNet(useSandbox);
            }
        }

        public void Dispose()
        {
        }

        #region Private Endpoints

        [Fact]
        public void GetBalances_HideZeros_Test()
        {
            var hideZeros = true;
            var balances = _service.GetBalances(hideZeros).Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalances_Test()
        {
            var balances = _service.GetBalances().Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalances_Symbol_Test()
        {
            var symbol = "BTC";
            var balances = _service.GetBalances(symbol).Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalances_Account_Test()
        {
            var type = AccountType.Main;
            var balances = _service.GetBalances(type).Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalances_Symbol_Account_Test()
        {
            var symbol = "BTC";
            var type = AccountType.Main;
            var balances = _service.GetBalances(symbol, type).Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalance_Test()
        {
            var balances = _service.GetBalances().Result;
            var id = balances.Select(b => b.Id).FirstOrDefault();

            var balance = _service.GetBalance(id).Result;

            Assert.NotNull(balance);
        }

        [Fact]
        public void CreateAccount_Test()
        {
            var symbol = "KCS";
            var type = AccountType.Trade;

            var accountId = _service.CreateAccount(symbol, type).Result;

            Assert.NotNull(accountId);
        }

        [Fact]
        public void GetAccountHistory_Test()
        {
            var symbol = "BTC";
            var balances = _service.GetBalances().Result;
            var accountId = balances.Where(b => b.Symbol.Equals(symbol) && b.Type.Equals("trade")).Select(b => b.Id).FirstOrDefault();
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow;

            var history = _service.GetAccountHistory(accountId, startDate, endDate).Result;

            Assert.NotNull(history);
        }

        [Fact]
        public void GetHolds_Test()
        {
            var symbol = "BTC";
            var balances = _service.GetBalances().Result;
            var accountId = balances.Where(b => b.Symbol.Equals(symbol) && b.Type.Equals("trade")).Select(b => b.Id).FirstOrDefault();

            var holds = _service.GetHolds(accountId).Result;

            // TODO: Check this one
            Assert.NotNull(holds);
        }

        [Fact]
        public void InnerTransfer_Test()
        {
            var symbol = "BTC";
            var balances = _service.GetBalances(symbol).Result;
            var fromId = balances.Where(b => b.Type == "main").Select(b => b.Id).FirstOrDefault();
            var amount = balances.Where(b => b.Type == "main").Select(b => b.Total).FirstOrDefault();
            var toId = balances.Where(b => b.Type == "trade").Select(b => b.Id).FirstOrDefault();

            var orderId = _service.InnerTransfer(fromId, toId, amount).Result;

            Assert.NotNull(orderId);
        }

        [Fact]
        public void PlaceLimitOrder_Test()
        {
            var orderParams = new LimitOrderParams
            {
                ClientOid = Guid.NewGuid().ToString(),
                Pair = "ETH-BTC",
                Price = 0.00002M,
                Side = Side.BUY,
                Size = 2
            };

            var orderId = _service.PlaceLimitOrder(orderParams).Result;

            Assert.NotNull(orderId);
        }

        [Fact]
        public void PlaceMarketOrder_Test()
        {
            var orderParams = new MarketOrderParams
            {
                ClientOid = Guid.NewGuid().ToString(),
                Pair = "ETH-BTC",
                Side = Side.BUY,
                Size = 2
            };

            var orderId = _service.PlaceMarketOrder(orderParams).Result;

            Assert.NotNull(orderId);
        }

        [Fact]
        public void PlaceStopOrder_Test()
        {
            var pair = "ETH-BTC";
            var side = Side.BUY;
            var price = 0.00002M;
            var stopPrice = 0.000025M;
            var quantity = 5;
            var stopType = StopType.ENTRY;

            // TODO need to fix this one
            var orderId = _service.PlaceStopOrder(pair, side, price, quantity, stopPrice, stopType).Result;

            Assert.NotNull(orderId);
        }

        [Fact]
        public void GetOpenOrders_Test()
        {
            var orders = _service.GetOpenOrders().Result;

            Assert.NotNull(orders);
        }

        #endregion Private Endpoints

        #region Public Endpoints

        [Fact]
        public void ValidateExchangeConfigured_Test()
        {
            var expected = _exchangeApi != null ? true : false;

            var result = _service.ValidateExchangeConfigured();

            Assert.True(expected == result);
        }

        [Fact]
        public void GetMarkets_Test()
        {
            var markets = _service.GetMarkets().Result;

            Assert.NotNull(markets);
        }

        [Fact]
        public void GetTradingPairDetails_Test()
        {
            var markets = _service.GetTradingPairDetails().Result;

            Assert.NotNull(markets);
        }

        [Fact]
        public void GetTicker_Test()
        {
            var pair = "ETH-BTC";

            var ticker = _service.GetTicker(pair).Result;

            Assert.NotNull(ticker);
        }

        [Fact]
        public void GetPartOrderBook_Test()
        {
            var pair = "ETH-BTC";

            var orderBook = _service.GetPartOrderBook(pair).Result;

            Assert.NotNull(orderBook);
        }

        [Fact]
        public void GetFullOrderBook_Test()
        {
            var pair = "ETH-BTC";

            var orderBook = _service.GetFullOrderBook(pair).Result;

            Assert.NotNull(orderBook);
        }

        [Fact]
        public void GetEntireOrderBook_Test()
        {
            var pair = "ETH-BTC";

            var orderBook = _service.GetEntireOrderBook(pair).Result;

            Assert.NotNull(orderBook);
        }

        [Fact]
        public void GetTradeHistory_Test()
        {
            var pair = "ETH-BTC";

            var history = _service.GetTradeHistory(pair).Result;

            Assert.NotNull(history);
        }

        [Fact]
        public void GetCandleStick_Simple_Test()
        {
            var pair = "ETH-BTC";
            var interval = Interval.OneH;
            var count = 5;

            var candlesticks = _service.GetCandlestick(pair, interval, count).Result;

            Assert.NotNull(candlesticks);
        }

        [Fact]
        public void Get24hrStats_Test()
        {
            var pair = "ETH-BTC";

            var stats = _service.Get24HrStats(pair).Result;

            Assert.NotNull(stats);
        }

        [Fact]
        public void GetCurrencies_Test()
        {
            var currencies = _service.GetCurrencies().Result;

            Assert.NotNull(currencies);
        }

        [Fact]
        public void GetCurrency_Test()
        {
            var symbol = "BTC";
            var currency = _service.GetCurrency(symbol).Result;

            Assert.NotNull(currency);
        }
        
        #endregion Public Endpoints
    }
}
