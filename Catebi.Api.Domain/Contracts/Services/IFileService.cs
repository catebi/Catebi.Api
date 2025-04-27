using Catebi.Api.Domain.Models;

namespace Catebi.Api.Domain.Contracts.Services;

public interface IFileService
{
    Task<FileStorage> SaveFileAsync(FileStorageDto file);
    string GenerateFileUrl(FileStorageDto file);
    Task<FileStorage?> GetFileByName(string fileName);
}
