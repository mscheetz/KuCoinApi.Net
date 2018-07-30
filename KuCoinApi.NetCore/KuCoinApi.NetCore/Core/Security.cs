using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KuCoinApi.NetCore.Core
{
    public class Security
    {
        /// <summary>
        /// Get KuCoin HMAC Signature
        /// </summary>
        /// <param name="secretKey">Api secret</param>
        /// <param name="message">Message to sign</param>
        /// <returns>Signature for request</returns>
        public string GetKuCoinHMACSignature(string secretKey, string message)
        {
            var msgString = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] msgBytes = Encoding.UTF8.GetBytes(msgString);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(msgBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
