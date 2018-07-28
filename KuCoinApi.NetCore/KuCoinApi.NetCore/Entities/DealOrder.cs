using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class DealOrder<T>
    {
        public int total { get; set; }
        public bool firstPage { get; set; }
        public bool lastPage { get; set; }
        public T datas { get; set; }
        public int currPageNo { get; set; }
        public int limit { get; set; }
        public int pageNos { get; set; }
    }
}
