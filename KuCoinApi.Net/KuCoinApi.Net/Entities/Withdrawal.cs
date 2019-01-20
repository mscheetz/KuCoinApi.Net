// -----------------------------------------------------------------------------
// <copyright file="Withdrawal" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 4:55:10 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class Withdrawal : Deposit
    {
        #region Properties

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        #endregion Properties
    }
}