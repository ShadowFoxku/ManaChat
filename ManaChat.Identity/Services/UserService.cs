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
    public class UserService(IUsersRepository userRepo, IRuneReaderManager readerManager) : IUserService
    {
        private readonly IUsersRepository UserRepository = userRepo;
        private readonly IRuneReaderManager ReaderManager = readerManager;

        public Task<Ritual<bool>> AreDetailsAvailable(string username, string email, string phoneNumber)
        {
            return UserRepository.AreDetailsAvailable(username, email, phoneNumber);
        }

        public Task<Ritual<User>> SearchUserByUsername(string username)
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
                            var identityResult = await CreateUserIdentity(savedUser.Id, savedUser.Username);

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

        public Task<Ritual<UserIdentity>> CreateUserIdentity(long userId, string name)
        {
            var identity = new UserIdentity
            {
                Name = name,
                UserId = userId,
                Default = false
            };

            return UserRepository.SaveUserIdentity(identity);
        }

        public Task<Ritual<(long, string)>> GetUserPassword(string username)
        {
            return UserRepository.GetUserByUsername(username).BindAsync((user) => Ritual<(long, string)>.Flow((user.Id, user.PasswordHash)));
        }

        public Task<Ritual<bool>> DeleteUser(long id)
        {
            return ReaderManager.RunInTransactionAsync(DatabaseConstants.IdentityDatabaseKey, () =>
                UserRepository.GetUserIdentities(id)
                    .BindAsync(async identities =>
                    {
                        var taskList = identities.Select(x => DeleteUserIdentity(x.Id));
                        await Task.WhenAll(taskList);
                        return await UserRepository.DeleteUser(id);
                    })
                );
        }

        public Task<Ritual<bool>> DeleteUserIdentity(long id)
        { 
            return UserRepository.DeleteUserIdentity(id);
        }

        public Task<Ritual<UserRelationship>> GetRelationshipBetweenUsers(long baseUserId, long recipientUserId)
        {
            return UserRepository.GetRelationshipBetweenUsers(baseUserId, recipientUserId);
        }

        public Task<Ritual<List<UserRelationship>>> GetRelationshipsForUser(long userId)
        {
            return UserRepository.GetUserRelationships(userId);
        }

        public Task<Ritual<User>> GetUser(long id)
        {
            return UserRepository.GetUser(id).BindAsync((user) => user.ToUser().ToRitual());
        }

        public Task<Ritual<List<UserIdentity>>> GetUserIdentities(long userId)
        {
            return UserRepository.GetUserIdentities(userId);
        }

        public Task<Ritual<UserWithIdentity>> GetUserWithIdentities(long id)
        {
            return UserRepository.GetUser(id)
                .BindAsync(async user => (await UserRepository.GetUserIdentities(id))
                    .Map(identities => UserWithIdentity.From(user.ToUser(), identities)));
        }

        public Task<Ritual<UserIdentity>> SaveUserIdentity(long userId, long id, string name, bool isDefault)
        {
            return ReaderManager.RunInTransactionAsync(DatabaseConstants.IdentityDatabaseKey, () => 
                UserRepository.GetUserIdentity(userId, id).BindAsync((identity) =>
                {
                    identity.Name = name;
                    identity.Default = isDefault;
                    if (isDefault != identity.Default)
                    {   // changing default - remove others
                        return UserRepository.GetUserIdentities(userId).BindAsync(async identities =>
                            {
                                var defaults = identities.Where(x => x.Default);
                                foreach(var d in defaults)
                                {   // should only be one, but this enforces the change
                                    d.Default = false;
                                    var ri = await UserRepository.SaveUserIdentity(d);
                                    if (!ri.IsFlowing)
                                        return Ritual<bool>.Tear(ri.GetTear()!);
                                }
                                return Ritual<bool>.Flow(true);
                            }).BindAsync((_) => UserRepository.SaveUserIdentity(identity));
                    }

                    return UserRepository.SaveUserIdentity(identity);
                })
            );
        }

        public Task<Ritual<UserRelationship>> SaveUserRelationship(long baseUser, long recipientUser, RelationshipType relationType, bool bookmarked)
        {
            return UserRepository.GetRelationshipBetweenUsers(baseUser, recipientUser).BindAsync((rel) =>
            {
                rel.RelationshipType = relationType;
                rel.Bookmarked = bookmarked;
                return UserRepository.SaveUserRelationship(rel);
            });
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
