using ManaChat.Core.Models.Identity;
using ManaFox.Core.Flow;

namespace ManaChat.Identity.Repositories
{
    public interface IRelationshipRepository
    {
        Task<Ritual<List<UserRelationship>>> GetUserRelationships(long userId);
        Task<Ritual<UserRelationship>> GetRelationshipBetweenUsers(long baseUserId, long recipientUserId);
        Task<Ritual<UserRelationship>> SaveUserRelationship(UserRelationship relationship);
    }
}
