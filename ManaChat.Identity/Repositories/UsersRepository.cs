using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Constants;
using ManaChat.Identity.Models;
using ManaFox.Core.Flow;
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

        public async Task<Ritual<List<User>>> SearchUserByUsername(string username)
        {
            await using var reader = await GetRuneReaderAsync();
            return await reader.QueryMultipleAsync<User>(IdentityDBConstants.StoredProcedures.SearchUserByUsername, CommandType.StoredProcedure, new { Username = username });
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

        public async Task<Ritual<bool>> UpdateUserSession(long sessionId, long userId, string token, DateTimeOffset expiresAt)
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
