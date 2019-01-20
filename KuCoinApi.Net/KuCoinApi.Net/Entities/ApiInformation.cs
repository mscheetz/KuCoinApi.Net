// -----------------------------------------------------------------------------
// <copyright file="ApiInformation" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 8:40:42 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class ApiInformation
    {
        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty(PropertyName = "apiSecret")]
        public string ApiSecret { get; set; }

        [JsonProperty(PropertyName = "apiPassword")]
        public string ApiPassword { get; set; }
    }
}
