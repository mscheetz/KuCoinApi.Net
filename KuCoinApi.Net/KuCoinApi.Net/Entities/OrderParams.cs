using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class OrderParams
    {
        public string ClientOid { get; set; }

        public OrderType Type { get; set; }

        public Side Side { get; set; }

        public string Pair { get; set; }

        public string Remark { get; set; }

        public SelfTradeProtect? SelfTradeProtect { get; set; }

        public decimal Size { get; set; }
    }
}
