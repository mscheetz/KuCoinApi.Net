using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class OrderBookResponse
    {
        [JsonProperty(PropertyName = "SELL")]
        public OrderBook[] sells { get; set; }
        [JsonProperty(PropertyName = "BUY")]
        public OrderBook[] buys { get; set; }
        public long timestamp { get; set; }
    }
}
