using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.NetCore.Entities
{
    public class Trending
    {
        public string coinPair { get; set; }
        public string[][] deals { get; set; }
    }
}
