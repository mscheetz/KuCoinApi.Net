using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class MarketOrderParams : OrderParams
    {
        public decimal Funds { get; set; }
    }
}
