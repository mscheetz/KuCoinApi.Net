// -----------------------------------------------------------------------------
// <copyright file="IdResponse" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/20/2019 9:09:55 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class IdResponse
    {
        #region Properties

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        #endregion Properties
    }
}