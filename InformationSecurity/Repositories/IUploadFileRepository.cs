using InformationSecurity.Models;

namespace InformationSecurity.Repositories
{
    public interface IUploadFileRepository
    {
        Task<IEnumerable<UploadFile>> GetUploadFilesAsync();
        Task<UploadFile> GetUploadFileAsync(Guid id);
        Task CreateUploadFileAsync(UploadFile uploadFile);
        Task DeleteUserPasswordAsync(Guid id);
        Task<IEnumerable<UploadFile>> GetUploadFilesByUserIdAsync(Guid userId);
    }
}
