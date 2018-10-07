using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class DepositWithdrawTransaction
    {
        public string address { get; set; }
        public decimal amount { get; set; }
        public string coinType { get; set; }
        public long confirmation { get; set; }
        public long createdAt { get; set; }
        public decimal fee { get; set; }
        public string oid { get; set; }
        public string outerWalletTxid { get; set; }
        public string remark { get; set; }
        public DWStatus status { get; set; }
        public DWType type { get; set; }
        public long? updatedAt { get; set; }
        public long? deletedAt { get; set; }
    }
}
