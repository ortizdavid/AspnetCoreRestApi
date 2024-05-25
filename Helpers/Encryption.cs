
using System.Buffers.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace AspNetCoreRestApi.Helpers
{
    public class Encryption
    {
        public static Guid GenerateUUID()
        {
            return Guid.NewGuid();
        }

        public static string? GenerateRandomToken(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenData = new byte[length];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

    }
    
}