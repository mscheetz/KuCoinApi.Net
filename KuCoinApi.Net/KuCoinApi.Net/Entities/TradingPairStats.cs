// -----------------------------------------------------------------------------
// <copyright file="TradingPairStats" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 9:48:42 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class TradingPairStats
    {
        #region Properties

        [JsonProperty(PropertyName = "symbol")]
        public string Pair { get; set; }

        [JsonProperty(PropertyName = "high")]
        public decimal High { get; set; }

        [JsonProperty(PropertyName = "vol")]
        public decimal Volume { get; set; }

        [JsonProperty(PropertyName = "volValue")]
        public decimal Volume24HrTotal { get; set; }

        public decimal Close
        {
            get
            {
                return this.Last;
            }
        }

        [JsonProperty(PropertyName = "last")]
        public decimal Last { get; set; }

        [JsonProperty(PropertyName = "low")]
        public decimal Low { get; set; }

        [JsonProperty(PropertyName = "buy")]
        public decimal Buy { get; set; }

        [JsonProperty(PropertyName = "sell")]
        public decimal Sell { get; set; }
        
        [JsonProperty(PropertyName = "changePrice")]
        public decimal ChangePrice { get; set; }

        [JsonProperty(PropertyName = "time")]
        public long Time { get; set; }

        [JsonProperty(PropertyName = "changeRate")]
        public decimal ChangeRate { get; set; }

        #endregion Properties
    }
}