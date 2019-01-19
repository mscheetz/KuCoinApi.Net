using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class FilledOrderDetail
    {
        public string oid { get; set; }
        public decimal dealPrice { get; set; }
        public string orderOid { get; set; }
        public string direction { get; set; }
        public decimal amount { get; set; }
        public decimal dealValue { get; set; }
        public long createdAt { get; set; }
    }
}
