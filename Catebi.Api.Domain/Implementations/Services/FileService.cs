using System.Security.Cryptography;
using Catebi.Api.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Catebi.Api.Domain.Implementations.Services;

public class FileService( CatebiContext        Context,
                          IHttpContextAccessor HttpContextAccessor,
                          ILogger<FileService> Logger) : IFileService
{
    private const int MaxFileSize = 10 * 1024 * 1024;
    private static readonly string[] AllowedMimeTypes = [ "image/jpeg", "image/png", "image/gif", "image/heic", "image/heif" ];

    public async Task<FileStorage> SaveFileAsync(FileStorageDto file)
    {
        Console.WriteLine($"🔍 FileService: Starting file validation for '{file.FileName}'");
        Console.WriteLine($"📋 FileService: File size: {file.Size} bytes, ContentType: '{file.ContentType}'");
        Console.WriteLine($"📝 FileService: Allowed MIME types: [{string.Join(", ", AllowedMimeTypes)}]");

        if (file.Size > MaxFileSize)
        {
            Console.WriteLine($"❌ FileService: File size {file.Size} exceeds maximum {MaxFileSize} bytes");
            throw new ArgumentException($"File size exceeds the maximum limit of {MaxFileSize} bytes.");
        }

        Console.WriteLine($"✅ FileService: File size validation passed");

        if (!AllowedMimeTypes.Contains(file.ContentType))
        {
            Console.WriteLine($"❌ FileService: ContentType '{file.ContentType}' not in allowed types");
            Console.WriteLine($"🔍 FileService: Exact comparison results:");
            foreach (var allowedType in AllowedMimeTypes)
            {
                Console.WriteLine($"   - '{allowedType}' == '{file.ContentType}': {allowedType == file.ContentType}");
            }
            throw new ArgumentException("Only image files (JPEG, PNG, GIF, HEIC) are allowed.");
        }

        Console.WriteLine($"✅ FileService: MIME type validation passed");

        var fileStorage = new FileStorage
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Size,
            Content = file.Data,
        };

        Console.WriteLine($"💾 FileService: About to save to database...");
        Context.FileStorage.Add(fileStorage);
        await Context.SaveChangesAsync();

        Console.WriteLine($"✅ FileService: File saved successfully with ID: {fileStorage.FileStorageId}");
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

    private async Task<FileStorage?> GetFileAsync(Guid id) => await Context.FileStorage.FindAsync(id);

    public async Task<string> ProcessFileUploadAsync(IFormFile file, string? prefix = null)
    {
        Logger.LogInformation($"📄 Received file: {file.FileName}");
        Logger.LogInformation($"📊 File details - Size: {file.Length} bytes, ContentType: '{file.ContentType}'");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var extension = Path.GetExtension(file.FileName);
        var fileName = Path.GetFileName(file.FileName) ?? $"{prefix ?? "file"}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        var fileSize = file.Length;
        var fileType = file.ContentType;

        Logger.LogInformation($"🔄 Processing file - Name: '{fileName}', Size: {fileSize}, Type: '{fileType}', Extension: '{extension}'");

        var fileRequest = new FileStorageDto
        {
            FileName = fileName,
            Size = fileSize,
            ContentType = fileType,
            Data = memoryStream.ToArray()
        };

        Logger.LogInformation($"💾 About to save file to storage...");
        var uploadedFile = await SaveFileAsync(fileRequest);
        Logger.LogInformation($"✅ File uploaded successfully: {uploadedFile.FileStorageId}");

        fileRequest.FileStorageId = uploadedFile.FileStorageId;
        var fileUrl = GenerateFileUrl(fileRequest);
        Logger.LogInformation($"🔗 Generated file URL: {fileUrl}");

        return fileUrl;
    }
}
