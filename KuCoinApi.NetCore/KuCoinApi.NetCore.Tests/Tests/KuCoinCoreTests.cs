using System;
using Xunit;

namespace KuCoinApi.NetCore.Tests.Tests
{
    public class KuCoinCoreTests : IDisposable
    {
        private KuCoinApi.NetCore.Core.Helper helper;
        private string[] markets;

        public void Dispose()
        {
        }

        public KuCoinCoreTests()
        {
            helper = new Core.Helper();
            markets = new string[] { "BTC", "ETH", "NEO", "USDT", "KCS", "TUSD", "PAX", "USDC" };
        }

        [Fact]
        public void TradingPairValidatorTests()
        {
            var pair = "ETHBTC";
            var expected = "ETH-BTC";

            var validated = helper.CreateDashedPair(pair, markets);

            Assert.Equal(expected, validated);

            pair = "BTCTUSD";
            expected = "BTC-TUSD";

            validated = helper.CreateDashedPair(pair, markets);

            Assert.Equal(expected, validated);

            pair = "NANONEO";
            expected = "NANO-NEO";

            validated = helper.CreateDashedPair(pair, markets);

            Assert.Equal(expected, validated);

            pair = "NANOUSDC";
            expected = "NANO-USDC";

            validated = helper.CreateDashedPair(pair, markets);

            Assert.Equal(expected, validated);
        }
    }
}
