using ManaChat.Core.Constants;
using ManaFox.Databases.Core.Interfaces;

namespace ManaChat.Identity.Repositories
{
    public class IdentityRepositoryBase(IRuneReaderManager runeFactory)
    {
        protected Task<IRitualRuneReader> GetRuneReaderAsync()
        {
            return runeFactory.GetRitualRuneReaderAsync(DatabaseConstants.IdentityDatabaseKey);
        }
    }
}
