// -----------------------------------------------------------------------------
// <copyright file="SecurityTests" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/20/2019 7:52:48 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Tests.Tests
{
    using KuCoinApi.Net.Core;
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using Xunit;

    #endregion Usings

    public class SecurityTests
    {
        [Fact]
        public void GetSignatureTest()
        {
            // Arrange
            var security = new Security();
            var apiSecret = "f03a5284-5c39-4aaa-9b20-dea10bdcf8e3";
            var timestamp = 1547015186532;
            var method = HttpMethod.Post;
            var endpoint = "/api/v1/deposit-addresses";
            var body = new SortedDictionary<string, object>();
            body.Add("currency", "BTC");
            var expected = "7QP/oM0ykidMdrfNEUmng8eZjg/ZvPafjIqmxiVfYu4=";

            // Act
            var signature = security.GetSignature(method, endpoint, timestamp, apiSecret, body);

            // Assert
            Assert.Equal(expected, signature);
        }
    }
}