using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class DeleteRequest
    {
        public string orderOid { get; set; }
        public string type { get; set; }
    }
}
