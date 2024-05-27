using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AspNetCoreRestApi.Helpers
{
    public class PasswordHelper 
    {
        // TODO
        public static string Hash(string? password)
        {
            if(string.IsNullOrEmpty(password)) {
                throw new ArgumentNullException("Password must have a value");
            }
            return password;
        }

        // TODO
        public static bool Verify(string? hashedPassword, string? providedPassword)
        {
            if(!string.IsNullOrEmpty(hashedPassword) || !string.IsNullOrEmpty(providedPassword))
            {
                return false;
            }
            return true;
        }   

    }
}
