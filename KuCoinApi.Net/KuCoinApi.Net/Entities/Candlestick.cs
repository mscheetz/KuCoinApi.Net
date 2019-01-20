// -----------------------------------------------------------------------------
// <copyright file="Candlestick" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 9:30:12 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using KuCoinApi.Net.Core;
    using Newtonsoft.Json;

    #endregion Usings

    [JsonConverter(typeof(Converter.ObjectToArrayConverter<OrderBook>))]
    public class Candlestick
    {
        #region Properties

        [JsonProperty(Order = 1)]
        public long StartTime { get; set; }

        [JsonProperty(Order = 2)]
        public decimal Open { get; set; }

        [JsonProperty(Order = 3)]
        public decimal Close { get; set; }

        [JsonProperty(Order = 4)]
        public decimal High { get; set; }

        [JsonProperty(Order = 5)]
        public decimal Low { get; set; }

        [JsonProperty(Order = 6)]
        public decimal Amount { get; set; }

        [JsonProperty(Order = 7)]
        public decimal Volume { get; set; }

        #endregion Properties
    }
}