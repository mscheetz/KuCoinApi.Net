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
        
        [JsonProperty(PropertyName = "changeRate")]
        public decimal ChangeRate { get; set; }
        
        [JsonProperty(PropertyName = "changePrice")]
        public decimal ChangePrice { get; set; }
        
        [JsonProperty(PropertyName = "open")]
        public decimal Open { get; set; }
        
        [JsonProperty(PropertyName = "close")]
        public decimal Close { get; set; }

        [JsonProperty(PropertyName = "high")]
        public decimal High { get; set; }

        [JsonProperty(PropertyName = "low")]
        public decimal Low { get; set; }

        [JsonProperty(PropertyName = "vol")]
        public decimal Volume { get; set; }

        [JsonProperty(PropertyName = "volValue")]
        public decimal VolumeValue { get; set; }

        #endregion Properties
    }
}