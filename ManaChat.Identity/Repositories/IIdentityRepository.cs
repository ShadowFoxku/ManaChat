using ManaChat.Core.Models.Identity;
using ManaFox.Core.Flow;

namespace ManaChat.Identity.Repositories
{
    public interface IIdentityRepository
    {
        Task<Ritual<List<UserIdentity>>> GetUserIdentities(long userId);
        Task<Ritual<UserIdentity>> SaveUserIdentity(UserIdentity identity);
        Task<Ritual<bool>> DeleteUserIdentity(long id);
        Task<Ritual<UserIdentity>> GetUserIdentity(long userid, long id);
    }
}
