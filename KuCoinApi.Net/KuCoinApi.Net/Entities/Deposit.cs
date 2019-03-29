// -----------------------------------------------------------------------------
// <copyright file="Deposit" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 4:32:22 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class Deposit
    {
        #region Properties

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "memo")]
        public string Memo { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "fee")]
        public decimal Fee { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "isInner")]
        public bool IsInner { get; set; }

        [JsonProperty(PropertyName = "walletTxId")]
        public string WalletTxId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public DepositStatus Status { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public long CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updatedAt")]
        public long UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "remark")]
        public string Remark { get; set; }

        #endregion Properties
    }
}