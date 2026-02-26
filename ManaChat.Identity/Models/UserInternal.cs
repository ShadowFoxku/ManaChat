using ManaChat.Core.Models.Identity;

namespace ManaChat.Identity.Models
{
    public class UserInternal
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = [];
        public byte[] PasswordSalt { get; set; } = [];
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public long? ExternalServerId { get; set; }

        public User ToUser()
        {
            return new User()
            {
                Id = Id,
                Username = Username,
                Email = Email,
                ExternalServerId = ExternalServerId,
            };
        }
    }
}
