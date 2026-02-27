using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Models;
using ManaFox.Core.Flow;

namespace ManaChat.Identity.Repositories
{
    public interface IUsersRepository
    {
        Task<Ritual<UserInternal>> GetUser(long id);
        Task<Ritual<UserInternal>> GetUserByUsername(string username);
        Task<Ritual<UserInternal>> SaveUser(UserInternal user);
        Task<Ritual<bool>> UpdateUserPassword(long id, string pwHash);
        Task<Ritual<bool>> DeleteUser(long id);
        Task<Ritual<bool>> UpdateUserSession(long sessionId, long userId, string token, DateTimeOffset expiresAt);
        Task<Ritual<Session>> GetUserSession(string token);
        Task<Ritual<bool>> AreDetailsAvailable(string username, string email, string phoneNumber);
        Task<Ritual<List<User>>> SearchUserByUsername(string username);
    }
}
