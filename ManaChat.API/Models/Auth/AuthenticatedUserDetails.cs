using ManaChat.Core.Models.Auth;

namespace ManaChat.API.Models.Auth
{
    public class AuthenticatedUserDetails : IAuthenticatedUserDetails
    {
        public long? UserId { get; set; } = null;
        public bool IsAuthenticated { get; set; } = false;
    }
}
