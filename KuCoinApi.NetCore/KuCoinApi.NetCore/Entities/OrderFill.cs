using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class OrderFill
    {
        public decimal amount { get; set; }
        public decimal dealValue { get; set; }
        public decimal fee { get; set; }
        public decimal dealPrice { get; set; }
        public decimal feeRate { get; set; }
    }
}
