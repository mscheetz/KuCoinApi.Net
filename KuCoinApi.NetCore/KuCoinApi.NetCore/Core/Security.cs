using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KuCoinApi.NetCore.Core
{
    public class Security
    {
        public string SHA256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public string GetKuCoinHMCACSignature(string secretKey, string message)
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
