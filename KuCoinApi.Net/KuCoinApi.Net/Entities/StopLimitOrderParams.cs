using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class StopLimitOrderParams : LimitOrderParams
    {

        public StopType? Stop { get; set; }

        public decimal StopPrice { get; set; }
    }
}
