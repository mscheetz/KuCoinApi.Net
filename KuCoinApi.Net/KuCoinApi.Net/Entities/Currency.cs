// -----------------------------------------------------------------------------
// <copyright file="Currency" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 9:53:08 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class Currency
    {
        #region Properties

        [JsonProperty(PropertyName = "currency")]
        public string Symbol { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "precision")]
        public int Precision { get; set; }

        #endregion Properties
    }
}