using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace KuCoinApi.Net.Core
{
    public class Security
    {
        /// <summary>
        /// Create signature for message
        /// </summary>
        /// <typeparam name="T">Type of data in post request</typeparam>
        /// <param name="method">HttpMethod being called</param>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="nonce">Current nonce</param>
        /// <param name="apiSecret">ApiSecret value</param>
        /// <param name="body">Request body message</param>
        /// <returns>String of signature</returns>
        public string GetSignature(HttpMethod method, string endpoint, long nonce, string apiSecret, SortedDictionary<string, object> body = null)
        {
            var timestamp = nonce.ToString();
            var callMethod = method.ToString().ToUpper();

            var jsonedBody = body != null && body.Count > 0
                ? JsonConvert.SerializeObject(body)
                : string.Empty;

            var sigString = $"{nonce}{callMethod}{endpoint}{jsonedBody}";

            var signature = HmacSha256(sigString, apiSecret);

            return signature;
        }

        /// <summary>
        /// Get HMAC Sha256 Signature
        /// </summary>
        /// <param name="message">Message to sign</param>
        /// <param name="secret">Api secret</param>
        /// <returns>Signature for request</returns>
        public string HmacSha256(string message, string secret)
        {
            var encoding = new ASCIIEncoding();
            var msgBytes = encoding.GetBytes(message);
            var secretBytes = encoding.GetBytes(secret);
            var hmac = new HMACSHA256(secretBytes);

            var hash = hmac.ComputeHash(msgBytes);

            return Convert.ToBase64String(hash);
        }
    }
}
