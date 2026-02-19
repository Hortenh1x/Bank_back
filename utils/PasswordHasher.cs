using Isopoh.Cryptography.Argon2;

namespace Bank_back.utils
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password, string pepper)
        {
            // 1. Combine the plain text password with your secret server Pepper
            string pepperedPassword = password + pepper;

            // 2. Argon2 automatically generates a secure 16-byte salt, 
            // runs the memory-hard math, and returns the full PHC string.
            // (By default, Isopoh uses Argon2id)
            return Argon2.Hash(pepperedPassword);
        }

        public static bool VerifyPassword(string inputPassword, string storedHash, string pepper)
        {
            // 1. Re-apply the Pepper to the incoming password
            string pepperedPassword = inputPassword + pepper;

            // 2. Argon2 automatically extracts the salt and rules from the storedHash
            // and securely compares them, protecting against Timing Attacks.
            return Argon2.Verify(storedHash, pepperedPassword);
        }
    }
}