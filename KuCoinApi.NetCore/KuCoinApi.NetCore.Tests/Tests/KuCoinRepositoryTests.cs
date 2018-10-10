﻿using KuCoinApi.NetCore.Entities;
using KuCoinApi.NetCore.Data.Interface;
using System;
using System.Linq;
using Xunit;
using KuCoinApi.NetCore.Data;
using FileRepository;

namespace KuCoinApi.NetCore.Tests
{
    public class KuCoinRepositoryTests : IDisposable
    {
        private ApiInformation _exchangeApi = null;
        private IKuCoinRepository _repo;
        private string configPath = "config.json";
        private string apiKey = string.Empty;
        private string apiSecret = string.Empty;

        public KuCoinRepositoryTests()
        {
            IFileRepository _fileRepo = new FileRepository.FileRepository();
            if (_fileRepo.FileExists(configPath))
            {
                _exchangeApi = _fileRepo.GetDataFromFile<ApiInformation>(configPath);
            }
            if (_exchangeApi != null || !string.IsNullOrEmpty(apiKey))
            {
                _repo = new KuCoinRepository(_exchangeApi.apiKey, _exchangeApi.apiSecret);
            }
            else
            {
                _repo = new KuCoinRepository();
            }
        }

        public void Dispose()
        {
        }

        [Fact]
        public void GetCandlesticksTest()
        {
            var interval = Interval.FifteenM;
            var symbol = "ETH-BTC";

            var sticks= _repo.GetCandlesticks(symbol, interval, 10).Result;

            Assert.True(sticks != null);
            Assert.True(sticks.close.Length > 0);
            Assert.True(sticks.open.Length > 0);
        }

        [Fact]
        public void GetAccountBalancesTest()
        {
            var balances= _repo.GetBalances().Result;

            Assert.True(balances != null);
        }

        [Fact]
        public void GetAccountBalancesLimitsTest()
        {
            var balances = _repo.GetBalances(5, 1).Result;

            Assert.True(balances != null);
        }

        [Fact]
        public void GetAccountBalanceOneCoinTest()
        {
            var symbol = "KCS";
            var balance = _repo.GetBalance(symbol).Result;

            Assert.True(balance != null);
        }

        [Fact]
        public void GetOrderBookTest()
        {
            var symbol = "ETH-BTC";

            var orderBook= _repo.GetOrderBook(symbol).Result;

            Assert.True(orderBook != null);
            Assert.True(orderBook.buys.Length > 0);
            Assert.True(orderBook.sells.Length > 0);
        }

        [Fact]
        public void GetTicksTest()
        {
            var ticks= _repo.GetTicks().Result;

            Assert.True(ticks != null);
        }

        [Fact]
        public void GetTickTest()
        {
            var symbol = "ETH-BTC";

            var tick= _repo.GetTick(symbol).Result;

            Assert.True(tick != null);
        }

        [Fact]
        public void GetOrdersTest()
        {
            var symbol = "DCC-BTC";

            var orders= _repo.GetOrders(symbol).Result;

            Assert.True(orders != null);
        }

        [Fact]
        public void GetOpenOrdersTest()
        {
            var symbol = "DCC-BTC";

            var orders= _repo.GetOpenOrders(symbol).Result;

            Assert.True(orders != null);
        }

        [Fact]
        public void PostTradeTest()
        {
            var symbol = "DCC-BTC";
            var tradeParams = new TradeParams
            {
                price = 0.00000400M,
                quantity = 3000,
                symbol = symbol,
                type = "BUY"
            };

            var orderDetail= _repo.PostTrade(tradeParams).Result;

            Assert.True(orderDetail != null);
        }

        [Fact]
        public void GetAndCancelOpenTradeTest()
        {
            var symbol = "DCC-BTC";

            var orders= _repo.GetOpenOrders(symbol).Result;

            Assert.True(orders != null);

            if (orders.openBuys.Length > 0)
            {
                var orderId = orders.openBuys[0].orderId;

                var cancelDetail= _repo.DeleteTrade(symbol, orderId, orders.openBuys[0].type).Result;

                Assert.True(cancelDetail != null);
            }

            if(orders.openSells.Length > 0)
            {
                var orderId = orders.openSells[0].orderId;

                var cancelDetail= _repo.DeleteTrade(symbol, orderId, orders.openSells[0].type).Result;

                Assert.True(cancelDetail != null);
            }
        }

        [Fact]
        public void GetMarketsTest()
        {
            var markets= _repo.GetMarkets().Result;

            Assert.NotNull(markets);
        }

        [Fact]
        public void GetTradingSymbolTickTest()
        {
            var ticks= _repo.GetTradingSymbolTick().Result;

            Assert.NotNull(ticks);
        }

        [Fact]
        public void GetTradingPairsTest()
        {
            var pairs= _repo.GetTradingPairs().Result;

            Assert.NotNull(pairs);
        }

        [Fact]
        public void GetCoinTest()
        {
            var symbol = "KCS";

            var coin = _repo.GetCoin(symbol).Result;

            Assert.NotNull(coin);
        }

        [Fact]
        public void GetCoinsTest()
        {
            var coins = _repo.GetCoins().Result;

            Assert.NotNull(coins);
        }

        [Fact]
        public void GetTrendingsTest()
        {
            var market = "USDT";

            var trendings = _repo.GetTrendings(market).Result;

            Assert.NotNull(trendings);
        }

        [Fact]
        public void GetDepositAddress()
        {
            var symbol = "NANO";

            var address = _repo.GetDepositAddress(symbol).Result;

            Assert.NotNull(address);
        }

        [Fact]
        public void WithdrawFundsTest()
        {
            var symbol = "XLM";
            var amount = 29.98M;
            var address = "GAHK7EEG2WWHVKDNT4CEQFZGKF2LGDSW2IVM4S5DP42RBW3K6BTODB4A";
            var memo = "1046303265";

            var confirmation = _repo.WithdrawFunds(symbol, amount, address, memo).Result;

            Assert.True(confirmation);
        }

        [Fact]
        public void GetDepositsTest()
        {
            var symbol = "XLM";
            var status = DWStatus.SUCCESS;

            var deposits = _repo.GetDeposits(symbol, status).Result;

            Assert.NotNull(deposits);
        }

        [Fact]
        public void GetWithdrawalsTest()
        {
            var symbol = "XLM";
            var status = DWStatus.SUCCESS;

            var deposits = _repo.GetWithdrawals(symbol, status).Result;

            Assert.NotNull(deposits);
        }
    }
}