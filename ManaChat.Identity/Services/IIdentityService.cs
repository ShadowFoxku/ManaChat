using ManaChat.Core.Models.Identity;
using ManaFox.Core.Flow;

namespace ManaChat.Identity.Services
{
    public interface IIdentityService
    {
        Task<Ritual<UserWithIdentity>> GetUserWithIdentities(long id);
        Task<Ritual<UserIdentity>> CreateUserIdentity(long userId, string name, bool isDefault);
        Task<Ritual<UserIdentity>> SaveUserIdentity(long userId, long id, string name, bool isDefault);
        Task<Ritual<UserIdentity>> GetUserIdentity(long userId, long id);
        Task<Ritual<List<UserIdentity>>> GetUserIdentities(long userId);
        Task<Ritual<bool>> DeleteUserIdentity(long id);
    }
}
