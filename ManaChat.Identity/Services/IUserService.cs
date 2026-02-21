using ManaChat.Core.Enums.Identity;
using ManaChat.Core.Models.Identity;
using ManaFox.Core.Flow;

namespace ManaChat.Identity.Services
{
    public interface IUserService
    {
        Task<Ritual<User>> GetUser(long id);
        Task<Ritual<User>> GetUserByUsername(string username);
        Task<Ritual<User>> CreateUser(string username, string email = "");
        Task<Ritual<User>> UpdateUser(long userId, string username, string email);
        Task<Ritual<bool>> UpdateUserPassword(long userId, string newHash);
        Task<Ritual<bool>> DeleteUser(long id);

        Task<Ritual<UserWithIdentity>> GetUserWithIdentities(long id);
        Task<Ritual<UserIdentity>> CreateUserIdentity(long userId, string name);
        Task<Ritual<UserIdentity>> SaveUserIdentity(long userId, long id, string name, bool isDefault);
        Task<Ritual<List<UserIdentity>>> GetUserIdentities(long userId);
        Task<Ritual<bool>> DeleteUserIdentity(long id);

        Task<Ritual<List<UserRelationship>>> GetRelationshipsForUser(long userId);
        Task<Ritual<UserRelationship>> GetRelationshipBetweenUsers(long baseUserId, long recipientUserId);
        Task<Ritual<UserRelationship>> SaveUserRelationship(long baseUser, long recipientUser, RelationshipType relationType, bool bookmarked);
    }
}
