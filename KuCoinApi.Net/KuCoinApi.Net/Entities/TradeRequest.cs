using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class TradeRequest
    {
        public string type { get; set; }
        public decimal price { get; set; }
        public decimal amount { get; set; }
    }
}
