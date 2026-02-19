using System;
using System.Security.Cryptography;

namespace Bank_back.utils
{
    public static class CryptoUtility
    {
        public static string GenerateSecureString(int byteLength)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(byteLength);
            var a = Convert.ToBase64String(randomBytes);
            return a;
        }
    }
}