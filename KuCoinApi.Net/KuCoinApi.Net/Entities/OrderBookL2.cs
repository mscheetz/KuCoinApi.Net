// -----------------------------------------------------------------------------
// <copyright file="OrderBookL2" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 9:07:27 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class OrderBookL2
    {
        #region Properties

        [JsonProperty(PropertyName = "sequence")]
        public long Sequence { get; set; }

        [JsonProperty(PropertyName = "bids")]
        public OrderBookDetailL2[] Bids { get; set; }

        [JsonProperty(PropertyName = "asks")]
        public OrderBookDetailL2[] Asks { get; set; }

        #endregion Properties
    }
}