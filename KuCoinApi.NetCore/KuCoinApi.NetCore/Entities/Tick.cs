using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class Tick
    {
        public string coinType { get; set; }
        public bool trading { get; set; }
        public decimal lastDealPrice { get; set; }
        public decimal buy { get; set; }
        public decimal sell { get; set; }
        public string coinTypePair { get; set; }
        public decimal sort { get; set; }
        public decimal feeRate { get; set; }
        public long volValue { get; set; }
        public decimal high { get; set; }
        public long datetime { get; set; }
        public long vol { get; set; }
        public decimal low { get; set; }
        public decimal changeRate { get; set; }
    }
}
