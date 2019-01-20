// -----------------------------------------------------------------------------
// <copyright file="Balance" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 11:46:37 AM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class Balance
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public decimal Total { get; set; }

        [JsonProperty(PropertyName = "available")]
        public decimal Available { get; set; }

        [JsonProperty(PropertyName = "holds")]
        public decimal Frozen { get; set; }
    }
}
