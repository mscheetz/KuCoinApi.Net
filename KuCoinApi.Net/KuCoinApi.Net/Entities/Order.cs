// -----------------------------------------------------------------------------
// <copyright file="Order" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 11:46:37 AM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class Order
    {
        #region Properties

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "symbol")]
        public string Pair { get; set; }

        [JsonProperty(PropertyName = "opType")]
        public string OpType { get; set; }

        [JsonProperty(PropertyName = "type")]
        public OrderType Type { get; set; }

        [JsonProperty(PropertyName = "side")]
        public Side Side { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "size")]
        public decimal Size { get; set; }

        [JsonProperty(PropertyName = "funds")]
        public decimal Funds { get; set; }

        [JsonProperty(PropertyName = "dealFunds")]
        public decimal DealFunds { get; set; }

        [JsonProperty(PropertyName = "dealSize")]
        public decimal DealSize { get; set; }

        [JsonProperty(PropertyName = "fee")]
        public decimal Fee { get; set; }

        [JsonProperty(PropertyName = "feeCurrency")]
        public string FeeCurrency { get; set; }

        [JsonProperty(PropertyName = "stp")]
        public SelfTradeProtect? SelfTradeProtect { get; set; }

        [JsonProperty(PropertyName = "stop")]
        public StopType? Stop { get; set; }

        [JsonProperty(PropertyName = "stopTriggered")]
        public bool StopTriggered { get; set; }

        [JsonProperty(PropertyName = "stopPrice")]
        public decimal StopPrice { get; set; }

        [JsonProperty(PropertyName = "timeInForce")]
        public TimeInForce TimeInForce { get; set; }

        [JsonProperty(PropertyName = "postOnly")]
        public bool PostOnly { get; set; }

        [JsonProperty(PropertyName = "hidden")]
        public bool Hidden { get; set; }

        [JsonProperty(PropertyName = "iceberge")]
        public bool Iceberge { get; set; }

        [JsonProperty(PropertyName = "visibleSize")]
        public decimal VisibleSize { get; set; }

        [JsonProperty(PropertyName = "cancelAfter")]
        public long CancelAfter { get; set; }

        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        [JsonProperty(PropertyName = "clientOid")]
        public string ClientOid { get; set; }

        [JsonProperty(PropertyName = "remark")]
        public string Remark { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public string Tags { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "cancelExist")]
        public bool CancelExist { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public long CreatedAt { get; set; }

        #endregion Properties
    }
}