// -----------------------------------------------------------------------------
// <copyright file="Ticker" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 9:07:27 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class Ticker
    {
        #region Properties

        [JsonProperty(PropertyName = "sequence")]
        public long Sequence { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "size")]
        public decimal Size { get; set; }

        [JsonProperty(PropertyName = "bestBid")]
        public decimal BestBid { get; set; }

        [JsonProperty(PropertyName = "bestBidSize")]
        public decimal BestBidSize { get; set; }

        [JsonProperty(PropertyName = "bestAsk")]
        public decimal BestAsk { get; set; }

        [JsonProperty(PropertyName = "bestAskSize")]
        public decimal BestAskSize { get; set; }

        #endregion Properties
    }
}