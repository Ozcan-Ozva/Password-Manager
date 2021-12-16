using InformationSecurity.Models;
using Microsoft.EntityFrameworkCore;

namespace InformationSecurity.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; init; }
        public DbSet<UploadFile> UploadFiles { get; init; }
        public DbSet<UserPassword> UserPasswords { get; init; }
        public DbSet<UserKey> UserKeys { get; init; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }
    }
}
