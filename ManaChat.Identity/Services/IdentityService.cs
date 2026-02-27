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
            var userIdentity = new UserIdentity()
            {
                UserId = userId,
                Id = id,
                Name = name,
                Default = isDefault
            };
            return IdentityRepository.SaveUserIdentity(userIdentity);
        }

        public Task<Ritual<bool>> DeleteUserIdentity(long id)
        {
            return IdentityRepository.DeleteUserIdentity(id);
        }

        public Task<Ritual<UserIdentity>> CreateUserIdentity(long userId, string name, bool isDefault)
        {
            var identity = new UserIdentity
            {
                Name = name,
                UserId = userId,
                Default = isDefault
            };

            return IdentityRepository.SaveUserIdentity(identity);
        }

        public Task<Ritual<UserIdentity>> GetUserIdentity(long userId, long id)
        {
            return IdentityRepository.GetUserIdentity(userId, id);
        }
    }
}
