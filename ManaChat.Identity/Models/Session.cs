namespace ManaChat.Identity.Models
{
    public class Session
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public string Token { get; set; } = null!;
    }
}
