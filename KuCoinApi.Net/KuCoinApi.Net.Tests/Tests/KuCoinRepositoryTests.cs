// -----------------------------------------------------------------------------
// <copyright file="KuCoinRepositoryTests" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/20/2019 7:52:48 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Tests
{
    #region Usings

    using KuCoinApi.Net.Entities;
    using System;
    using System.Linq;
    using Xunit;
    using FileRepository;

    #endregion Usings

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
            IFileRepository _fileRepo = new FileRepository();
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
            var balances = _service.GetBalances().Result;
            var accountId = balances.Where(b => b.Type.Equals("trade")).Select(b => b.Id).FirstOrDefault();
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
            var price = 0.0002M;
            var stopPrice = 0.00021M;
            var quantity = 5;
            var stopType = StopType.ENTRY;

            var orderId = _service.PlaceStopOrder(pair, side, price, quantity, stopPrice, stopType).Result;

            Assert.NotNull(orderId);
        }

        [Fact]
        public void GetOpenOrders_Test()
        {
            var orders = _service.GetOpenOrders().Result;

            Assert.NotNull(orders);
        }

        [Fact]
        public void CancelOpenOrders_Test()
        {
            var orders = _service.GetOpenOrders().Result;
            var ids = orders.Data.Select(d => d.Id).ToArray();

            var cancel = _service.CancelOrder(ids[0]).Result;
            
            Assert.NotNull(cancel);
        }

        [Fact]
        public void CancelAllOpenOrders_Test()
        {
            var orders = _service.GetOpenOrders().Result;

            var cancel = _service.CancelAllOrders().Result;

            Assert.NotNull(cancel);
        }

        [Fact]
        public void GetOrders_Test()
        {
            var orders = _service.GetOrders().Result;

            Assert.NotNull(orders);
        }

        [Fact]
        public void GetOrder_Id_Test()
        {
            var orders = _service.GetOrders().Result;
            var id = orders.Data.Select(d => d.Id).FirstOrDefault();

            var order = _service.GetOrder(id).Result;

            Assert.NotNull(order);
        }

        [Fact]
        public void GetHistoricOrders_Test()
        {
            var start = new DateTime(2018, 10, 1);
            var end = new DateTime(2018, 12, 31);

            var orders = _service.GetHistoricOrders(start, end).Result;

            Assert.NotNull(orders);
        }

        [Fact]
        public void GetOrders_Complete_Test()
        {
            var orders = _service.GetOrders(OrderStatus.DONE).Result;

            Assert.NotNull(orders);
        }

        [Fact]
        public void GetFills_Test()
        {
            var fills = _service.GetFills().Result;

            Assert.NotNull(fills);
        }

        [Fact]
        public void CreateDepositAddress_Test()
        {
            var symbol = "BTC";
            var address = _service.CreateDepositAddress(symbol).Result;

            Assert.NotNull(address);
        }

        [Fact]
        public void GetDepositAddress_Test()
        {
            var symbol = "BTC";
            var address = _service.GetDepositAddress(symbol).Result;
            if(address == null)
            {
                var newAddress = _service.CreateDepositAddress(symbol).Result;

                address = _service.GetDepositAddress(symbol).Result;
            }

            Assert.NotNull(address);
        }

        [Fact]
        public void GetDeposits_Test()
        {
            var symbol = "BTC";
            var history = _service.GetDepositHistory(symbol).Result;

            Assert.NotNull(history);
        }

        [Fact]
        public void GetWithdrawals_Test()
        {
            var symbol = "BTC";
            var history = _service.GetWithdrawalHistory(symbol).Result;

            Assert.NotNull(history);
        }

        [Fact]
        public void GetWithdrawalQuota_Test()
        {
            var symbol = "BTC";
            var history = _service.GetWithdrawalQuota(symbol).Result;

            Assert.NotNull(history);
        }

        [Fact]
        public void Withdrawal_Test()
        {
            var symbol = "BTC";
            var address = "13VFW1D9ZPhXHLQGeZ4Vyf39LJpFbhopNR";
            var amount = 0.01M;

            var withdrawal = _service.Withdrawal(symbol: symbol, address: address, amount: amount).Result;

            Assert.NotNull(withdrawal);
        }

        [Fact]
        public void CancelWithdrawal_Test()
        {
            var id = "";

            var withdrawal = _service.CancelWithdrawal(id).Result;

            Assert.NotNull(withdrawal);
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
        public void GetTradingPairs_Test()
        {
            var markets = _service.GetTradingPairs().Result;

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
        public void GetAllTickers_Test()
        {
            var tickers = _service.GetAllTickers().Result;

            Assert.NotNull(tickers);
        }

        [Fact]
        public void Get24hrStats_Test()
        {
            var pair = "ETH-BTC";

            var stats = _service.Get24HrStats(pair).Result;

            Assert.NotNull(stats);
        }

        [Fact]
        public void GetMarkets_Test()
        {
            var stats = _service.GetMarkets().Result;

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

        [Fact]
        public void GetFiatPrice_Test()
        {
            var fiats = _service.GetFiatPrice().Result;

            Assert.NotNull(fiats);
        }

        #endregion Public Endpoints

        #region Websocket

        [Fact]
        public void GetPublicChannels_Test()
        {
            var channels = _service.GetPublicChannels().Result;

            Assert.NotNull(channels);
        }

        [Fact]
        public void GetPrivateChannels_Test()
        {
            var channels = _service.GetPrivateChannels().Result;

            Assert.NotNull(channels);
        }

        #endregion Websocket
    }
}
