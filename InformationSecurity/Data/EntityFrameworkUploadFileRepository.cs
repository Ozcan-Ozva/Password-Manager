using InformationSecurity.Models;
using InformationSecurity.Repositories;

namespace InformationSecurity.Data
{
    public class EntityFrameworkUploadFileRepository : IUploadFileRepository
    {
        private readonly ApplicationDbContext _context;

        public EntityFrameworkUploadFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateUploadFileAsync(UploadFile uploadFile)
        {
            await _context.AddAsync(uploadFile);
        }

        public Task DeleteUserPasswordAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UploadFile> GetUploadFileAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UploadFile>> GetUploadFilesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UploadFile>> GetUploadFilesByUserIdAsync(Guid userId)
        {
            return await Task.FromResult(_context.UploadFiles.Where(uploadFile => uploadFile.UserPasswordId == userId).ToList());
        }
    }
}
