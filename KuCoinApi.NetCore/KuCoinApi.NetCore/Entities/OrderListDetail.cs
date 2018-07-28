using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class OrderListDetail
    {
        public decimal amount { get; set; }
        public string coinType { get; set; }
        public decimal dealValue { get; set; }
        public decimal fee { get; set; }
        public string dealDirection { get; set; }
        public string coinTypePair { get; set; }
        public string oid { get; set; }
        public decimal dealPrice { get; set; }
        public string orderOid { get; set; }
        public decimal feeRate { get; set; }
        public long createdAt { get; set; }
        public long id { get; set; }
        public string direction { get; set; }
    }
}
