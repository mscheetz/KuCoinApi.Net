// -----------------------------------------------------------------------------
// <copyright file="AllTickerResponse" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="3/3/2019 5:29:09 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;
    using System.Collections.Generic;

    #endregion Usings

    public class AllTickerResponse
    {
        #region Properties

        [JsonProperty(PropertyName = "time")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "ticker")]
        public List<TradingPairStats> Tickers { get; set; }

        #endregion Properties
    }
}