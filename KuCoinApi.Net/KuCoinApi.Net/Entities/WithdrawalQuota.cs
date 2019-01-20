// -----------------------------------------------------------------------------
// <copyright file="WithdrawalQuota" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 8:33:26 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class WithdrawalQuota
    {
        #region Properties

        [JsonProperty(PropertyName = "currency")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "availableAmount")]
        public decimal AvailableAmount { get; set; }

        [JsonProperty(PropertyName = "remainAmount")]
        public decimal RemainAmount { get; set; }

        [JsonProperty(PropertyName = "withdrawMinSize")]
        public decimal WithdrawMinSize { get; set; }

        [JsonProperty(PropertyName = "limitBTCAmount")]
        public decimal LimitBTCAmount { get; set; }

        [JsonProperty(PropertyName = "InnerWithdrawMinFee")]
        public decimal InnerWithdrawMinFee { get; set; }

        [JsonProperty(PropertyName = "isWithdrawEnabled")]
        public bool IsWithdrawEnabled { get; set; }

        [JsonProperty(PropertyName = "withdrawMinFee")]
        public decimal WithdrawMinFee { get; set; }

        [JsonProperty(PropertyName = "precision")]
        public int Precision { get; set; }

        #endregion Properties
    }
}