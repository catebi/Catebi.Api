using System.Security.Cryptography;
using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Catebi.Api.Domain.Implementations.Services;

public class FileService( CatebiContext        Context,
                          IHttpContextAccessor HttpContextAccessor) : IFileService
{
    private const int MaxFileSize = 10 * 1024 * 1024;
    private static readonly string[] AllowedMimeTypes = [ "image/jpeg", "image/png", "image/gif" ];

    public async Task<FileStorage> SaveFileAsync(FileStorageDto file)
    {
        if (file.Size > MaxFileSize)
        {
            throw new ArgumentException($"File size exceeds the maximum limit of {MaxFileSize} bytes.");
        }

        if (!AllowedMimeTypes.Contains(file.ContentType))
        {
            throw new ArgumentException("Only image files (JPEG, PNG, GIF) are allowed.");
        }

        var fileStorage = new FileStorage
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Size,
            Content = file.Data,
        };

        Context.FileStorage.Add(fileStorage);
        await Context.SaveChangesAsync();

        return fileStorage;
    }

    public string GenerateFileUrl(FileStorageDto file)
    {
        var baseUrl = $"{HttpContextAccessor.HttpContext?.Request.Scheme}://{HttpContextAccessor.HttpContext?.Request.Host}";
        var token = GenerateToken(file.FileStorageId);
        var fileExtension = Path.GetExtension(file.FileName);
        return $"{baseUrl}/file/{token}{fileExtension}";
    }

    public async Task<FileStorage?> GetFileByName(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        var (fileId, expiryTicks) = DecodeToken(fileName);

        if (fileId == Guid.Empty)
        {
            return null;
        }

        var expiry = new DateTime(expiryTicks, DateTimeKind.Utc);
        if (expiry < DateTime.UtcNow)
        {
            return null;
        }

        var file = await GetFileAsync(fileId);
        if (file == null)
        {
            return null;
        }

        return file;
    }

    private static string GenerateToken(Guid id)
    {
        var expiry = DateTime.UtcNow.AddMinutes(5);
        var tokenData = $"{id}:{expiry.Ticks}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(tokenData));
        var hash = Convert.ToBase64String(hashBytes);
        var composed = $"{tokenData}:{hash}";
        return Uri.EscapeDataString(composed);
    }

    /// <summary>
    /// example token: "zoKTDXCsC2vm3bfQhq5d+WQKrIYkl2NzpefNukdw6b0=.jpg"
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static (Guid FileId, long ExpiryTicks) DecodeToken(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        // split off extension
        var extension    = Path.GetExtension(fileName);
        var tokenEncoded = Path.GetFileNameWithoutExtension(fileName);
        var token        = Uri.UnescapeDataString(tokenEncoded);

        // parse token
        var parts = token.Split(':');
        if (parts.Length != 3)
            throw new ArgumentException("Invalid token format.");

        if (!Guid.TryParse(parts[0], out var fileId))
            throw new ArgumentException("Invalid file identifier.");

        if (!long.TryParse(parts[1], out var ticks))
            throw new ArgumentException("Invalid expiry timestamp.");

        // verify hash
        var payload       = $"{parts[0]}:{parts[1]}";
        var expectedHash  = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));

        if (!expectedHash.Equals(parts[2], StringComparison.Ordinal))
            throw new ArgumentException("Invalid token signature.");

        return (fileId, ticks);
    }

    private async Task<FileStorage?> GetFileAsync(Guid id)
    {
        return await Context.FileStorage.FindAsync(id);
    }
}
