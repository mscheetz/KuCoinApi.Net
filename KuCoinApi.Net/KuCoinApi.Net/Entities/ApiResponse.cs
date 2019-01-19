using System;
using System.Collections.Generic;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
}
