using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
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
