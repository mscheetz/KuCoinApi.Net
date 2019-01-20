// -----------------------------------------------------------------------------
// <copyright file="AccountHold" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 11:46:37 AM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class AccountHold
    {
        [JsonProperty(PropertyName = "currency")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "holdAmount")]
        public decimal Amount { get; set; }
        
        [JsonProperty(PropertyName = "bizType")]
        public string BusinessType { get; set; }

        [JsonProperty(PropertyName = "orderId")]
        public decimal OrderId { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public long CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updatedAt")]
        public long UpdatedAt { get; set; }
    }
}
