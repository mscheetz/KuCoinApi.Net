using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class DeleteResponse
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string data { get; set; }
    }
}
