using ManaChat.Core.Constants;
using ManaChat.Core.Enums.Identity;
using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Models;
using ManaChat.Identity.Repositories;
using ManaFox.Core.Flow;
using ManaFox.Databases.Core.Interfaces;
using ManaFox.Extensions.Flow;

namespace ManaChat.Identity.Services
{
    public class UserService(IUsersRepository userRepo, IIdentityService identityService, IRuneReaderManager readerManager) : IUserService
    {
        private readonly IUsersRepository UserRepository = userRepo;
        private readonly IIdentityService IdentityService = identityService;
        private readonly IRuneReaderManager ReaderManager = readerManager;

        public Task<Ritual<bool>> AreDetailsAvailable(string username, string email, string phoneNumber)
        {
            return UserRepository.AreDetailsAvailable(username, email, phoneNumber);
        }

        public Task<Ritual<List<User>>> SearchUserByUsername(string username)
        {
            return UserRepository.SearchUserByUsername(username);
        }

        public async Task<Ritual<User>> CreateUser(string username, string email, string phoneNumber, string password)
        {
            var user = new UserInternal
            {
                Username = username,
                Email = email,
                PhoneNumber = phoneNumber,
            };

            return await AreDetailsAvailable(username, email, phoneNumber).BindAsync(async (res) =>
                {
                    if (!res)
                        return Ritual<User>.Tear("Username, email, or phone number is already in use.");

                    return await ReaderManager.RunInTransactionAsync(DatabaseConstants.IdentityDatabaseKey, async () =>
                    {
                        return await UserRepository.SaveUser(user).BindAsync(async (savedUser) =>
                        {
                            var identityResult = await IdentityService.CreateUserIdentity(savedUser.Id, savedUser.Username, true);

                            return identityResult.Map(_ => savedUser.ToUser());
                        }).BindAsync(async (user) =>
                        {
                            await UpdateUserPassword(user.Id, password);
                            return Ritual<User>.Flow(user);
                        });
                    });
                }
            );
        }

        public Task<Ritual<(long, string)>> GetUserPassword(string username)
        {
            return UserRepository.GetUserByUsername(username).BindAsync((user) => Ritual<(long, string)>.Flow((user.Id, user.PasswordHash)));
        }

        public Task<Ritual<bool>> UpdateUserSession(long sessionId, long userId, string token, DateTimeOffset expiresAt)
        {
            return UserRepository.UpdateUserSession(sessionId, userId, token, expiresAt);
        }

        public Task<Ritual<bool>> DeleteUser(long id)
        {
            return ReaderManager.RunInTransactionAsync(DatabaseConstants.IdentityDatabaseKey, () =>
                IdentityService.GetUserIdentities(id)
                    .BindAsync(async identities =>
                    {
                        var taskList = identities.Select(x => IdentityService.DeleteUserIdentity(x.Id));
                        await Task.WhenAll(taskList);
                        return await UserRepository.DeleteUser(id);
                    })
                );
        }

        public Task<Ritual<User>> GetUser(long id)
        {
            return UserRepository.GetUser(id).BindAsync((user) => user.ToUser().ToRitual());
        }
        

        public Task<Ritual<User>> UpdateUser(long userId, string username, string email)
        {
            return UserRepository.GetUser(userId).BindAsync(async (user) =>
            {
                user.Username = username;
                user.Email = email;
                await UserRepository.SaveUser(user);
                return user.ToUser().ToRitual();
            });
        }

        public Task<Ritual<bool>> UpdateUserPassword(long userId, string pwHash)
        {
            return UserRepository.UpdateUserPassword(userId, pwHash);
        }
    }
}
