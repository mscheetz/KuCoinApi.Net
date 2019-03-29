// -----------------------------------------------------------------------------
// <copyright file="Bullet" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="3/28/2019 8:25:51 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities.Websocket
{
    #region Usings

    using Newtonsoft.Json;

    #endregion Usings

    public class Bullet
    {
        #region Properties

        [JsonProperty(PropertyName = "instanceServers")]
        public InstanceServer[] InstanceServers { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        #endregion Properties
    }
}