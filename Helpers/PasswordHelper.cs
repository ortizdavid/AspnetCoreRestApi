using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AspNetCoreRestApi.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string? password)
        {
           
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); 
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

           
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

         
            byte[] hashBytes = new byte[salt.Length + Convert.FromBase64String(hashed).Length];
            Array.Copy(salt, 0, hashBytes, 0, salt.Length);
            Array.Copy(Convert.FromBase64String(hashed), 0, hashBytes, salt.Length, Convert.FromBase64String(hashed).Length);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool CheckPassword(string storedHash, string password)
        {
            // Get the stored hash bytes
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract the salt from the stored hash
            byte[] salt = new byte[128 / 8];
            Array.Copy(hashBytes, 0, salt, 0, salt.Length);

            // Extract the hash from the stored hash
            byte[] storedPasswordHash = new byte[hashBytes.Length - salt.Length];
            Array.Copy(hashBytes, salt.Length, storedPasswordHash, 0, storedPasswordHash.Length);

           
            string hashedInputPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

        
            byte[] hashedInputPasswordBytes = Convert.FromBase64String(hashedInputPassword);

            // Compare the hashes
            for (int i = 0; i < storedPasswordHash.Length; i++)
            {
                if (storedPasswordHash[i] != hashedInputPasswordBytes[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
