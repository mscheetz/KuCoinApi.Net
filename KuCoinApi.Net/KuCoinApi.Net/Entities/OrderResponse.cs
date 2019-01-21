// -----------------------------------------------------------------------------
// <copyright file="OrderResponse" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/20/2019 9:15:52 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class OrderResponse
    {
        #region Properties

        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        #endregion Properties
    }
}