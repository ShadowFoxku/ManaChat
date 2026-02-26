using Konscious.Security.Cryptography;
using ManaChat.Core.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ManaChat.API.Helpers
{
    public static class PasswordHelpers
    {
        private const int HashSize = 32;
        private const int SaltSize = 16;

        public static (byte[] hash, byte[] salt) HashPassword(string password, PasswordSettings settings)
        {
            byte[] salt = GenerateSalt();
            byte[] hash = ComputeHash(password, salt, settings);
            return (hash, salt);
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt, PasswordSettings settings)
        {
            byte[] newHash = ComputeHash(password, storedSalt, settings);
            return CryptographicOperations.FixedTimeEquals(newHash, storedHash);
        }

        private static byte[] ComputeHash(string password, byte[] salt, PasswordSettings settings)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = settings.DegreeOfParallelism,
                Iterations = settings.Iterations,
                MemorySize = settings.MemorySize
            };

            return argon2.GetBytes(HashSize);
        }

        private static byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(SaltSize);
        }
    }
}
