using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class DepositWithdawDetail<T>: DealOrder<T>
    {
        public int[] navigatePageNos { get; set; }
        public string userOid { get; set; }
        public string direction { get; set; }
        public long startRow { get; set; }
        public string coinType { get; set; }
        public string type { get; set; }
        public string status { get; set; }
    }
}
