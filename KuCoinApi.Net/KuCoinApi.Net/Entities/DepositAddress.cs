// -----------------------------------------------------------------------------
// <copyright file="DepositAddress" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 12:52:09 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    using Newtonsoft.Json;
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class DepositAddress
    {
        #region Properties

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "memo")]
        public string Memo { get; set; }

        #endregion Properties
    }
}