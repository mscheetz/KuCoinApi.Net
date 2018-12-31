using KuCoinApi.NetCore.Core;
using Newtonsoft.Json;

namespace KuCoinApi.NetCore.Entities
{
    [JsonConverter(typeof(Converter.ObjectToArrayConverter<OpenOrder>))]
    public class OpenOrder
    {
        [JsonProperty(Order = 1)]
        public long timestamp { get; set; }
        [JsonProperty(Order = 2)]
        public string type { get; set; }
        [JsonProperty(Order = 3)]
        public decimal price { get; set; }
        [JsonProperty(Order = 4)]
        public decimal quantity { get; set; }
        [JsonProperty(Order = 5)]
        public decimal filledQuantity { get; set; }
        [JsonProperty(Order = 6)]
        public string orderId { get; set; }
    }
}
