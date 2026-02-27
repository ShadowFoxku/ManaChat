using ManaChat.Core.Enums.Identity;
using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Constants;
using ManaFox.Core.Flow;
using ManaFox.Databases.Core;
using ManaFox.Databases.Core.Interfaces;
using ManaFox.Extensions.Flow;
using System.Data;

namespace ManaChat.Identity.Repositories
{
    public class RelationshipRepository(IRuneReaderManager manager) : IdentityRepositoryBase(manager), IRelationshipRepository
    {
        public async Task<Ritual<List<UserRelationship>>> GetUserRelationships(long userId)
        {
            await using var reader = await GetRuneReaderAsync();
            return await reader.QueryMultipleAsync<UserRelationship>(IdentityDBConstants.StoredProcedures.GetUserRelationships, CommandType.StoredProcedure, new { UserId = userId });
        }

        public async Task<Ritual<UserRelationship>> SaveUserRelationship(UserRelationship relationship)
        {
            await using var reader = await GetRuneReaderAsync();
            return (await reader.QuerySingleAsync<long>(IdentityDBConstants.StoredProcedures.SaveUserRelationship, CommandType.StoredProcedure, new
            {
                relationship.Id,
                relationship.UserId,
                relationship.RecipientUserId,
                RelationshipType = (long)relationship.RelationshipType,
                relationship.Bookmarked
            })).Map(id =>
            {
                if (relationship.Id == 0)
                    relationship.Id = id;
                return relationship;
            });
        }

        public async Task<Ritual<UserRelationship>> GetRelationshipBetweenUsers(long baseUserId, long recipientUserId)
        {
            await using var reader = await GetRuneReaderAsync();

            return await reader.QuerySingleAsync<UserRelationship>(IdentityDBConstants.StoredProcedures.GetUserRelationships, CommandType.StoredProcedure,
                new
                {
                    UserId = baseUserId,
                    RecipientUserId = recipientUserId
                }).RecoverAsync(async (tear) =>
                {
                    if (tear.Code == DBErrorCodes.QueryReturnedNoResults)
                    {
                        var relationship = new UserRelationship()
                        {
                            UserId = baseUserId,
                            RecipientUserId = recipientUserId,
                            RelationshipType = RelationshipType.None,
                            Bookmarked = true,
                        };
                        return await SaveUserRelationship(relationship);
                    }
                    return Ritual<UserRelationship>.Tear(tear);
                });
        }
    }
}
