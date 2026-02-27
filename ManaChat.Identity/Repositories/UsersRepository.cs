using ManaChat.Core.Enums.Identity;
using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Constants;
using ManaChat.Identity.Models;
using ManaFox.Core.Flow;
using ManaFox.Databases.Core;
using ManaFox.Databases.Core.Interfaces;
using ManaFox.Extensions.Flow;
using System.Data;

namespace ManaChat.Identity.Repositories
{
    public class UsersRepository(IRuneReaderManager runeFactory) : IdentityRepositoryBase(runeFactory), IUsersRepository
    {
        public async Task<Ritual<UserInternal>> GetUser(long id)
        {
            await using var reader = await GetRuneReaderAsync();
            return await reader.QuerySingleAsync<UserInternal>(IdentityDBConstants.StoredProcedures.GetUserById, CommandType.StoredProcedure, new { UserId = id });
        }

        public async Task<Ritual<UserInternal>> SaveUser(UserInternal user)
        {
            await using var reader = await GetRuneReaderAsync();

            var res = (await reader.QuerySingleAsync<long>(IdentityDBConstants.StoredProcedures.SaveUser, CommandType.StoredProcedure, new
            {
                user.Id,
                user.Username,
                user.Email,
                user.PhoneNumber,
                ServerId = user.ExternalServerId
            })).Map(uId =>
            {
                if (user.Id == 0)
                    user.Id = uId;
                return user;
            });

            return res;
        }

        public async Task<Ritual<bool>> UpdateUserPassword(long id, string pwHash)
        {
            await using var reader = await GetRuneReaderAsync();
            return (await reader.ExecuteAsync(IdentityDBConstants.StoredProcedures.UpdateUserPassword, CommandType.StoredProcedure, new
            {
                UserId = id,
                PasswordHash = pwHash
            })).Map(result => result > 0);
        }

        public async Task<Ritual<UserInternal>> GetUserByUsername(string username)
        {
            await using var reader = await GetRuneReaderAsync();
            return await reader.QuerySingleOrDefaultAsync<UserInternal>(IdentityDBConstants.StoredProcedures.GetUserByUsername, CommandType.StoredProcedure, new { Username = username });
        }

        public async Task<Ritual<User>> SearchUserByUsername(string username)
        {
            await using var reader = await GetRuneReaderAsync();
            return await reader.QuerySingleOrDefaultAsync<User>(IdentityDBConstants.StoredProcedures.SearchUserByUsername, CommandType.StoredProcedure, new { Username = username });
        }

        public async Task<Ritual<bool>> DeleteUser(long id)
        {
            await using var reader = await GetRuneReaderAsync();
            return (await reader.ExecuteAsync(IdentityDBConstants.StoredProcedures.DeleteUser, CommandType.StoredProcedure, new { Id = id })).Map(result => result > 0);
        }

        public async Task<Ritual<Session>> GetUserSession(string token)
        {
            await using var reader = await GetRuneReaderAsync();
            return await reader.QuerySingleOrDefaultAsync<Session>(IdentityDBConstants.StoredProcedures.GetUserSessionByToken, CommandType.StoredProcedure, new { Token = token });
        }

        public async Task<Ritual<bool>> UpdateUserSession(long sessionId, long userId, string token, DateTime expiresAt)
        {
            await using var reader = await GetRuneReaderAsync();
            return (await reader.ExecuteAsync(IdentityDBConstants.StoredProcedures.UpdateUserSession, CommandType.StoredProcedure, new
            {
                Id = sessionId,
                UserId = userId,
                Token = token,
                StartedAt = DateTimeOffset.UtcNow,
                ExpiresAt = expiresAt
            })).Map(result => result > 0);
        }

        public async Task<Ritual<List<UserIdentity>>> GetUserIdentities(long userId)
        {
            await using var reader = await GetRuneReaderAsync();
            return await reader.QueryMultipleAsync<UserIdentity>(IdentityDBConstants.StoredProcedures.GetUserIdentitiesForUser, CommandType.StoredProcedure, new { UserId = userId });
        }

        public async Task<Ritual<UserIdentity>> GetUserIdentity(long userId, long id)
        {
            await using var reader = await GetRuneReaderAsync();
            return await reader.QuerySingleAsync<UserIdentity>(IdentityDBConstants.StoredProcedures.GetUserIdentity, CommandType.StoredProcedure, new { UserId = userId, Id = id });
        }

        public async Task<Ritual<UserIdentity>> SaveUserIdentity(UserIdentity identity)
        {
            await using var reader = await GetRuneReaderAsync();
            return (await reader.QuerySingleAsync<long>(IdentityDBConstants.StoredProcedures.SaveUserIdentity, CommandType.StoredProcedure, new
            {
                identity.Id,
                identity.UserId,
                identity.Default,
                identity.Name
            })).Map(iId =>
            {
                if (identity.Id == 0)
                    identity.Id = iId;
                return identity;
            });
        }

        public async Task<Ritual<bool>> DeleteUserIdentity(long id)
        {
            await using var reader = await GetRuneReaderAsync();
            return (await reader.ExecuteAsync(IdentityDBConstants.StoredProcedures.DeleteUserIdentity, CommandType.StoredProcedure, new { Id = id }))
                .Map(result => result > 0);
        }

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

        public async Task<Ritual<bool>> AreDetailsAvailable(string username, string email, string phoneNumber)
        {
            await using var reader = await GetRuneReaderAsync();
            var result = await reader.QuerySingleAsync<int>(IdentityDBConstants.StoredProcedures.VerifyDetailsAvailable, CommandType.StoredProcedure, new
            {
                Username = username,
                Email = string.IsNullOrWhiteSpace(email) ? null : email,
                PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber
            });
            return result.Map((res) => res > 0);
        }
    }
}
