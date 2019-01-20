// -----------------------------------------------------------------------------
// <copyright file="TradingPairDetail" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 8:58:28 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class TradingPairDetail
    {
        #region Properties

        [JsonProperty(PropertyName = "symbol")]
        public string Pair { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonProperty(PropertyName = "quoteCurrency")]
        public string QuoteCurrency { get; set; }

        [JsonProperty(PropertyName = "baseMinSize")]
        public decimal BaseMinSize { get; set; }

        [JsonProperty(PropertyName = "quoteMinSize")]
        public decimal QuoteMinSize { get; set; }

        [JsonProperty(PropertyName = "baseMaxSize")]
        public decimal BaseMaxSize { get; set; }

        [JsonProperty(PropertyName = "quoteMaxSize")]
        public decimal QuoteMaxSize { get; set; }

        [JsonProperty(PropertyName = "baseIncrement")]
        public decimal BaseIncrement { get; set; }

        [JsonProperty(PropertyName = "quoteIncrement")]
        public decimal QuoteIncrement { get; set; }

        [JsonProperty(PropertyName = "priceIncrement")]
        public decimal PriceIncrement { get; set; }

        [JsonProperty(PropertyName = "enableTrading")]
        public bool EnableTrading { get; set; }

        #endregion Properties
    }
}