using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class OpenOrderResponse<T>
    {
        [JsonProperty(PropertyName = "SELL")]
        public T[] openSells { get; set; }
        [JsonProperty(PropertyName = "BUY")]
        public T[] openBuys { get; set; }
    }
}
