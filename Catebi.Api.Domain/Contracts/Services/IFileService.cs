using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Catebi.Api.Domain.Contracts.Services;

public interface IFileService
{
    Task<FileStorage> SaveFileAsync(FileStorageDto file);
    string GenerateFileUrl(FileStorageDto file);
    Task<FileStorage?> GetFileByName(string fileName);
    Task<string> ProcessFileUploadAsync(IFormFile file, string? prefix = null);
}
