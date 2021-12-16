using InformationSecurity.Repositories;

namespace InformationSecurity.Data
{
    public class EntityFrameworkUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public EntityFrameworkUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
