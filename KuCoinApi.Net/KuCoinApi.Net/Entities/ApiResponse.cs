// -----------------------------------------------------------------------------
// <copyright file="ApiResponse" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/20/2019 8:12:48 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class ApiResponse<T>
    {
        #region Properties

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }

        #endregion Properties
    }
}