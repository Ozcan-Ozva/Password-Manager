using InformationSecurity.Models;

namespace InformationSecurity.Repositories
{
    public interface IUserPasswordsRepository
    {
        Task<IEnumerable<UserPassword>> GetUserPasswordsAsync();
        Task<UserPassword> GetUserPasswordAsync(Guid id);
        Task CreateUserPasswordAsync(UserPassword userPassword);
        Task UpdateUserPasswordAsync(UserPassword userPassword);
        Task DeleteUserPasswordAsync(Guid id);
    }
}
