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
        private IKuCoinRepository _repo;
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
                _repo = new KuCoinRepository(_exchangeApi, useSandbox);
            }
            else
            {
                _repo = new KuCoinRepository(useSandbox);
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
            var balances = _repo.GetBalances(hideZeros).Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalances_Test()
        {
            var balances = _repo.GetBalances().Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalances_Symbol_Test()
        {
            var symbol = "BTC";
            var balances = _repo.GetBalances(symbol).Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalances_Account_Test()
        {
            var type = AccountType.Main;
            var balances = _repo.GetBalances(type).Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalances_Symbol_Account_Test()
        {
            var symbol = "BTC";
            var type = AccountType.Main;
            var balances = _repo.GetBalances(symbol, type).Result;

            Assert.NotNull(balances);
        }

        [Fact]
        public void GetBalance_Test()
        {
            var balances = _repo.GetBalances().Result;
            var id = balances.Select(b => b.Id).FirstOrDefault();

            var balance = _repo.GetBalance(id).Result;

            Assert.NotNull(balance);
        }

        [Fact]
        public void CreateAccount_Test()
        {
            var symbol = "KCS";
            var type = AccountType.Trade;

            var accountId = _repo.CreateAccount(symbol, type).Result;

            Assert.NotNull(accountId);
        }

        [Fact]
        public void GetAccountHistory_Test()
        {
            var balances = _repo.GetBalances().Result;
            var accountId = balances.Select(b => b.Id).FirstOrDefault();
            var startDate = DateTime.UtcNow.AddDays(-100);
            var endDate = DateTime.UtcNow;

            var history = _repo.GetAccountHistory(accountId, startDate, endDate).Result;

            Assert.NotNull(history);
        }

        [Fact]
        public void GetHolds_Test()
        {
            var balances = _repo.GetBalances().Result;
            var accountId = balances.Select(b => b.Id).FirstOrDefault();

            var holds = _repo.GetHolds(accountId).Result;

            // TODO: Check this one
            Assert.NotNull(holds);
        }

        [Fact]
        public void InnerTransfer_Test()
        {
            var symbol = "BTC";
            var balances = _repo.GetBalances(symbol).Result;
            var fromId = balances.Where(b => b.Type == "main").Select(b => b.Id).FirstOrDefault();
            var amount = balances.Where(b => b.Type == "main").Select(b => b.Total).FirstOrDefault();
            var toId = balances.Where(b => b.Type == "trade").Select(b => b.Id).FirstOrDefault();

            var orderId = _repo.InnerTransfer(fromId, toId, amount).Result;

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

            var orderId = _repo.PlaceLimitOrder(orderParams).Result;

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

            var orderId = _repo.PlaceMarketOrder(orderParams).Result;

            Assert.NotNull(orderId);
        }

        [Fact]
        public void PlaceStopOrder_Test()
        {
            var orderParams = new StopLimitOrderParams
            {
            };

            var orderId = _repo.PlaceStopOrder(orderParams).Result;

            Assert.NotNull(orderId);
        }

        #endregion Private Endpoints

        #region Public Endpoints

        [Fact]
        public void GetMarkets_Test()
        {
            var markets = _repo.GetMarkets().Result;

            Assert.NotNull(markets);
        }

        [Fact]
        public void GetTradingPairDetails_Test()
        {
            var markets = _repo.GetTradingPairDetails().Result;

            Assert.NotNull(markets);
        }

        #endregion Public Endpoints
    }
}
