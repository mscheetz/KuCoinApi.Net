using KuCoinApi.NetCore.Core;
using Newtonsoft.Json;

namespace KuCoinApi.NetCore.Entities
{
    public class OpenOrderDetail
    {
        public string oid { get; set; }
        public string type { get; set; }
        public string userOid { get; set; }
        public string coinType { get; set; }
        public string coinTypePair { get; set; }
        public string direction { get; set; }
        public decimal price { get; set; }
        public decimal dealAmount { get; set; }
        public decimal pendingAmount { get; set; }
        public long createdAt { get; set; }
        public long updatedAt { get; set; }
    }
}
