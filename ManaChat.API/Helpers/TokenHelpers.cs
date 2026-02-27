using System.Security.Cryptography;

namespace ManaChat.API.Helpers
{
    public static class TokenHelpers
    {
        public static (string token, string hash) GenerateNewToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(tokenBytes);
            var hash = Convert.ToBase64String(SHA256.HashData(tokenBytes));
            return (token, hash);
        }

        public static string HashToken(string token)
        {
            var tokenBytes = Convert.FromBase64String(token);
            return Convert.ToBase64String(SHA256.HashData(tokenBytes));
        }
    }
}
