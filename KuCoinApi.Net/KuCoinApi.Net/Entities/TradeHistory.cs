// -----------------------------------------------------------------------------
// <copyright file="TradeHistory" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 9:24:22 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class TradeHistory
    {
        #region Properties

        [JsonProperty(PropertyName = "sequence")]
        public long Sequence { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "size")]
        public decimal Size { get; set; }

        [JsonProperty(PropertyName = "side")]
        public Side Side { get; set; }

        [JsonProperty(PropertyName = "time")]
        public long Timestamp { get; set; }

        #endregion Properties
    }
}