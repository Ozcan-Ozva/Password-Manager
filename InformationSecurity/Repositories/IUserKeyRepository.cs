using InformationSecurity.Models;

namespace InformationSecurity.Repositories
{
    public interface IUserKeyRepository
    {
        Task<IEnumerable<UserKey>> GetUserKeysAsync();
        Task<UserKey> GetUserKeyAsync(Guid id);
        Task CreateUserKeyAsync(UserKey userKey);
        Task UpdateUserKeyAsync(UserKey userKey);
        Task DeleteUserKeyAsync(Guid id);
        byte[] Encrypt(string plainText, string key);
    }
}
