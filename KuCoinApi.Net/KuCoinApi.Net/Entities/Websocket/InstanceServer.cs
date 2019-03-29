// -----------------------------------------------------------------------------
// <copyright file="InstanceServer" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="3/28/2019 8:24:01 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities.Websocket
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class InstanceServer
    {
        #region Properties

        [JsonProperty(PropertyName = "pingInterval")]
        public long PingInterval { get; set; }

        [JsonProperty(PropertyName = "endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty(PropertyName = "protocol")]
        public string Protocol { get; set; }

        [JsonProperty(PropertyName = "encrypt")]
        public bool Encrypt { get; set; }

        [JsonProperty(PropertyName = "pingTimeout")]
        public long PingTimeout { get; set; }

        #endregion Properties
    }
}