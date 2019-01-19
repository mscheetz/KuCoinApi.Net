using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class ChartValue
    {
        public string s { get; set; }
        [JsonProperty(PropertyName = "c")]
        public decimal[] close { get; set; }
        [JsonProperty(PropertyName = "t")]
        public long[] timestamp { get; set; }
        [JsonProperty(PropertyName = "v")]
        public decimal[] volume { get; set; }
        [JsonProperty(PropertyName = "h")]
        public decimal[] high { get; set; }
        [JsonProperty(PropertyName = "l")]
        public decimal[] low { get; set; }
        [JsonProperty(PropertyName = "o")]
        public decimal[] open { get; set; }
    }
}
