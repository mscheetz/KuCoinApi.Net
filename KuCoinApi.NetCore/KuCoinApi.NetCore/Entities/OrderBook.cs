using KuCoinApi.NetCore.Core;
using Newtonsoft.Json;

namespace KuCoinApi.NetCore.Entities
{
    [JsonConverter(typeof(Converter.ObjectToArrayConverter<OrderBook>))]
    public class OrderBook
    {
        [JsonProperty(Order = 1)]
        public decimal price { get; set; }
        [JsonProperty(Order = 2)]
        public decimal quantity { get; set; }
        [JsonProperty(Order = 3)]
        public decimal pairTotal { get; set; }
    }
}
