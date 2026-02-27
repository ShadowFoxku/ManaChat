using ManaChat.Core.Enums.Identity;
using ManaChat.Core.Models.Identity;
using ManaFox.Core.Flow;

namespace ManaChat.Identity.Services
{
    public interface IUserService
    {
        Task<Ritual<User>> GetUser(long id);
        Task<Ritual<User>> CreateUser(string username, string email, string phoneNumber, string password);
        Task<Ritual<User>> UpdateUser(long userId, string username, string email);
        Task<Ritual<bool>> UpdateUserPassword(long userId, string pwHash);
        Task<Ritual<bool>> DeleteUser(long id);
        Task<Ritual<bool>> AreDetailsAvailable(string username, string email, string phoneNumber);
        Task<Ritual<(long, string)>> GetUserPassword(string userName);
        Task<Ritual<List<User>>> SearchUserByUsername(string username);
        Task<Ritual<bool>> UpdateUserSession(long sessionId, long userId, string token, DateTimeOffset expiresAt);
    }
}
