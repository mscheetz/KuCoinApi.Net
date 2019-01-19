using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class LimitOrderParams : OrderParams
    {
        public decimal Price { get; set; }

        public TimeInForce? TimeInForce { get; set; }

        public long CancelAfter { get; set; }

        public bool PostOnly { get; set; }
    }
}
