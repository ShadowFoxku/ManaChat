using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Models;
using ManaFox.Core.Flow;

namespace ManaChat.Identity.Repositories
{
    public interface IUsersRepository
    {
        Task<Ritual<UserInternal>> GetUser(long id);
        Task<Ritual<User>> GetUserByUsername(string username);
        Task<Ritual<UserInternal>> SaveUser(UserInternal user);
        Task<Ritual<bool>> UpdateUserPassword(long id, string passwordHash);
        Task<Ritual<bool>> DeleteUser(long id);
        
        Task<Ritual<List<UserIdentity>>> GetUserIdentities(long userId);
        Task<Ritual<UserIdentity>> SaveUserIdentity(UserIdentity identity);
        Task<Ritual<bool>> DeleteUserIdentity(long id);
        Task<Ritual<UserIdentity>> GetUserIdentity(long userid, long id);
        
        Task<Ritual<List<UserRelationship>>> GetUserRelationships(long userId);
        Task<Ritual<UserRelationship>> GetRelationshipBetweenUsers(long baseUserId, long recipientUserId);
        Task<Ritual<UserRelationship>> SaveUserRelationship(UserRelationship relationship);
    }
}
