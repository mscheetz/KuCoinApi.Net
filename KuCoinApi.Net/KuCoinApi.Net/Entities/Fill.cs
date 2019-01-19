// -----------------------------------------------------------------------------
// <copyright file="Fill" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 12:40:16 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    using Newtonsoft.Json;
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class Fill
    {
        #region Properties

        [JsonProperty(PropertyName = "symbol")]
        public string Pair { get; set; }

        [JsonProperty(PropertyName = "tradeId")]
        public string TradeId { get; set; }

        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        [JsonProperty(PropertyName = "counterOrderId")]
        public string CounterOrderId { get; set; }

        [JsonProperty(PropertyName = "side")]
        public Side Side { get; set; }

        [JsonProperty(PropertyName = "liquidity")]
        public string Liquidity { get; set; }

        [JsonProperty(PropertyName = "forceTaker")]
        public bool ForceTaker { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "size")]
        public decimal Size { get; set; }

        [JsonProperty(PropertyName = "funds")]
        public decimal Funds { get; set; }

        [JsonProperty(PropertyName = "fee")]
        public decimal Fee { get; set; }

        [JsonProperty(PropertyName = "feeRate")]
        public decimal FeeRate { get; set; }

        [JsonProperty(PropertyName = "feeCurrency")]
        public string FeeCurrency { get; set; }

        [JsonProperty(PropertyName = "stop")]
        public string Stop { get; set; }

        [JsonProperty(PropertyName = "type")]
        public OrderType Type { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public long CreatedAt { get; set; }

        #endregion Properties
    }
}