namespace ManaChat.Core.Models.Identity
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long? ExternalServerId { get; set; }
        public bool Deleted { get; set; }
    }

    public class UserWithIdentity : User
    {
        public List<UserIdentity> Identities { get; set; } = [];

        public static UserWithIdentity From(User user, IEnumerable<UserIdentity> identities)
        {
            return new UserWithIdentity()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                ExternalServerId = user.ExternalServerId,
                Identities = [.. identities]
            };
        }
    }
}
