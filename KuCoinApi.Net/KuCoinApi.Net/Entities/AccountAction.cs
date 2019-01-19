using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class AccountAction
    {
        [JsonProperty(PropertyName = "currency")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "fee")]
        public decimal Fee { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public decimal Balance { get; set; }

        [JsonProperty(PropertyName = "bizType")]
        public string BusinessType { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public string Direction { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public long CreatedAt { get; set; }

        [JsonProperty(PropertyName = "context")]
        public Dictionary<string, object> Context { get; set; }

    }
}
