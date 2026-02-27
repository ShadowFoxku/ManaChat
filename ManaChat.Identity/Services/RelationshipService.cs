using ManaChat.Core.Enums.Identity;
using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Repositories;
using ManaFox.Core.Flow;
using ManaFox.Extensions.Flow;

namespace ManaChat.Identity.Services
{
    public class RelationshipService(IRelationshipRepository relationRepo) : IRelationshipService
    {
        private readonly IRelationshipRepository RelationshipRepository = relationRepo;
        public Task<Ritual<UserRelationship>> SaveUserRelationship(long baseUser, long recipientUser, RelationshipType relationType, bool bookmarked)
        {
            return RelationshipRepository.GetRelationshipBetweenUsers(baseUser, recipientUser).BindAsync((rel) =>
            {
                rel.RelationshipType = relationType;
                rel.Bookmarked = bookmarked;
                return RelationshipRepository.SaveUserRelationship(rel);
            });
        }

        public Task<Ritual<UserRelationship>> GetRelationshipBetweenUsers(long baseUserId, long recipientUserId)
        {
            return RelationshipRepository.GetRelationshipBetweenUsers(baseUserId, recipientUserId);
        }

        public Task<Ritual<List<UserRelationship>>> GetRelationshipsForUser(long userId)
        {
            return RelationshipRepository.GetUserRelationships(userId);
        }
    }
}
