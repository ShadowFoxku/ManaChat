namespace ManaChat.Core.Models.Identity
{
    public class UserIdentity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public bool Default { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Deleted { get; set; }
    }
}
