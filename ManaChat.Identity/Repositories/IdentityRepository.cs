using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Constants;
using ManaFox.Core.Flow;
using ManaFox.Databases.Core.Interfaces;
using ManaFox.Extensions.Flow;
using System.Data;

namespace ManaChat.Identity.Repositories
{
    public class IdentityRepository(IRuneReaderManager manager) : IdentityRepositoryBase(manager), IIdentityRepository
    {
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
    }
}
