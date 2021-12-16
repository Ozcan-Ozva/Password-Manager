using InformationSecurity.Models;
using InformationSecurity.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InformationSecurity.Data
{
    public class EntityFrameworkUserPasswordRepository : IUserPasswordsRepository
    {
        private readonly ApplicationDbContext _context;

        public EntityFrameworkUserPasswordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateUserPasswordAsync(UserPassword userPassword)
        {
            await _context.UserPasswords.AddAsync(userPassword);
        }

        public Task DeleteUserPasswordAsync(Guid id)
        {
            var userPassword = _context.UserPasswords.SingleOrDefault(userPassword => userPassword.Id == id);
            if (userPassword == null)
                return null;
            _context.UserPasswords.Remove(userPassword);
            return Task.CompletedTask;
        }

        public async Task<UserPassword> GetUserPasswordAsync(Guid id)
        {
            var userPassword = await _context.UserPasswords
                .Include(userPassword => userPassword.User)
                .Include(userPassword => userPassword.UploadFiles)
                .SingleOrDefaultAsync(userPassword => userPassword.Id == id);
            if (userPassword == null)
                return null;
            return await Task.FromResult(userPassword);
        }

        public async Task<IEnumerable<UserPassword>> GetUserPasswordsAsync()
        {
            return await Task.FromResult(
                _context.UserPasswords.Include(userPassword => userPassword.User)
                .ToList()
                );
        }

        public Task UpdateUserPasswordAsync(UserPassword userPassword)
        {
            throw new NotImplementedException();
        }
    }
}
