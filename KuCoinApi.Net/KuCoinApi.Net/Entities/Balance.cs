using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
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
