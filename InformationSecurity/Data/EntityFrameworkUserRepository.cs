using InformationSecurity.Models;
using InformationSecurity.Repositories;

namespace InformationSecurity.Data
{
    public class EntityFrameworkUserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public EntityFrameworkUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task DeleteUserAsync(Guid id)
        {
            var user = _context.Users.SingleOrDefault(user => user.Id == id);
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return await Task.FromResult(user);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await Task.FromResult(_context.Users.ToList());
        }

        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
