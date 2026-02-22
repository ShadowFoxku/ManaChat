namespace ManaChat.Core.Models.Auth
{
    public interface IAuthenticatedUserDetails
    {
        long? UserId { get; }
        bool IsAuthenticated { get; }
    }
}
