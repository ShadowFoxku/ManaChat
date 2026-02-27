using ManaChat.Core.Enums.Identity;
using ManaChat.Core.Models.Identity;
using ManaFox.Core.Flow;

namespace ManaChat.Identity.Services
{
    public interface IRelationshipService
    {
        Task<Ritual<List<UserRelationship>>> GetRelationshipsForUser(long userId);
        Task<Ritual<UserRelationship>> GetRelationshipBetweenUsers(long baseUserId, long recipientUserId);
        Task<Ritual<UserRelationship>> SaveUserRelationship(long baseUser, long recipientUser, RelationshipType relationType, bool bookmarked);
    }
}
