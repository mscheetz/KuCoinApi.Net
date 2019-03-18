// -----------------------------------------------------------------------------
// <copyright file="HistoricOrder" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 11:46:37 AM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class HistoricOrder
    {
        #region Properties
        
        [JsonProperty(PropertyName = "symbol")]
        public string Pair { get; set; }

        [JsonProperty(PropertyName = "dealPrice")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "dealValue")]
        public decimal Total { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Quantity { get; set; }

        [JsonProperty(PropertyName = "fee")]
        public decimal Fee { get; set; }

        [JsonProperty(PropertyName = "side")]
        public Side Side { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public long CreatedAt { get; set; }

        #endregion Properties
    }
}