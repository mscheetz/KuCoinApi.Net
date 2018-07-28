using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class OpenOrderResponse
    {
        [JsonProperty(PropertyName = "SELL")]
        public OpenOrderDetail[] openSells { get; set; }
        [JsonProperty(PropertyName = "BUY")]
        public OpenOrderDetail[] openBuys { get; set; }
    }
}
