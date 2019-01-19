using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class FilledOrderResponse
    {
        [JsonProperty(PropertyName = "datas")]
        public FilledOrderDetail[] orderDetails { get; set; }
        public long total { get; set; }
        public long limit { get; set; }
        public long pageNos { get; set; }
        public int currPageNo { get; set; }
        public int[] navigatePageNos { get; set; }
        public string userOid { get; set; }
        public string direction { get; set; }
        public long startRow { get; set; }
        public bool firstPage { get; set; }
        public bool lastPage { get; set; }
    }
}
