namespace ManaChat.Core.Helpers
{
    public static class RandomStrings
    {
        public static string GenerateRandomDeletedNameString()
        {
            return Guid.NewGuid().ToString().Split("-").Last();
        }
    }
}
