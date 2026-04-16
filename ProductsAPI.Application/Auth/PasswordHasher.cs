using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ProductsAPI.Application.Auth
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt)));
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            var expected = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(password + parts[0])));

            return parts[1] == expected;
        }
    }
}
