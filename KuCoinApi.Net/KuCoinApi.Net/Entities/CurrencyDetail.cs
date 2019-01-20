// -----------------------------------------------------------------------------
// <copyright file="CurrencyDetail" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 9:53:08 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class CurrencyDetail : Currency
    {
        #region Properties

        [JsonProperty(PropertyName = "withdrawalMinSize")]
        public decimal WithdrawalMinSize { get; set; }

        [JsonProperty(PropertyName = "withdrawalMinFee")]
        public decimal WithdrawalMinFee { get; set; }

        [JsonProperty(PropertyName = "isWithdrawEnabled")]
        public bool WithdrawEnabled { get; set; }

        [JsonProperty(PropertyName = "isDepositEnabled")]
        public bool DepositEnabled { get; set; }

        #endregion Properties
    }
}