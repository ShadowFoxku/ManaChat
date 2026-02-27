using ManaChat.Core.Constants;
using ManaChat.Core.Models.Identity;
using ManaChat.Identity.Repositories;
using ManaFox.Core.Flow;
using ManaFox.Databases.Core.Interfaces;
using ManaFox.Extensions.Flow;

namespace ManaChat.Identity.Services
{
    public class IdentityService(IIdentityRepository identityRepository, IUsersRepository userRepo, IRuneReaderManager readerManager) : IIdentityService
    {
        private readonly IIdentityRepository IdentityRepository = identityRepository;
        private readonly IUsersRepository UserRepository = userRepo;
        private readonly IRuneReaderManager ReaderManager = readerManager;

        public Task<Ritual<List<UserIdentity>>> GetUserIdentities(long userId)
        {
            return IdentityRepository.GetUserIdentities(userId);
        }

        public Task<Ritual<UserWithIdentity>> GetUserWithIdentities(long id)
        {
            return UserRepository.GetUser(id)
                .BindAsync(async user => (await IdentityRepository.GetUserIdentities(id))
                    .Map(identities => UserWithIdentity.From(user.ToUser(), identities)));
        }

        public Task<Ritual<UserIdentity>> SaveUserIdentity(long userId, long id, string name, bool isDefault)
        {
            return ReaderManager.RunInTransactionAsync(DatabaseConstants.IdentityDatabaseKey, () =>
                IdentityRepository.GetUserIdentity(userId, id).BindAsync((identity) =>
                {
                    identity.Name = name;
                    identity.Default = isDefault;
                    if (isDefault != identity.Default)
                    {   // changing default - remove others
                        return IdentityRepository.GetUserIdentities(userId).BindAsync(async identities =>
                        {
                            var defaults = identities.Where(x => x.Default);
                            foreach (var d in defaults)
                            {   // should only be one, but this enforces the change
                                d.Default = false;
                                var ri = await IdentityRepository.SaveUserIdentity(d);
                                if (!ri.IsFlowing)
                                    return Ritual<bool>.Tear(ri.GetTear()!);
                            }
                            return Ritual<bool>.Flow(true);
                        }).BindAsync((_) => IdentityRepository.SaveUserIdentity(identity));
                    }

                    return IdentityRepository.SaveUserIdentity(identity);
                })
            );
        }

        public Task<Ritual<bool>> DeleteUserIdentity(long id)
        {
            return IdentityRepository.DeleteUserIdentity(id);
        }

        public Task<Ritual<UserIdentity>> CreateUserIdentity(long userId, string name)
        {
            var identity = new UserIdentity
            {
                Name = name,
                UserId = userId,
                Default = false
            };

            return IdentityRepository.SaveUserIdentity(identity);
        }
    }
}
