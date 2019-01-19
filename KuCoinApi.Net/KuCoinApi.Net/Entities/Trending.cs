using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class Trending
    {
        public string coinPair { get; set; }
        public string[][] deals { get; set; }
    }
}
