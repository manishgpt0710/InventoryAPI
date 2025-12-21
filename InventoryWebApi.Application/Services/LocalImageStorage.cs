using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace InventoryWebApi.Application.Services;

public sealed class LocalImageStorage : IImageStorage
{
    private readonly LocalStorageOptions _options;

    public LocalImageStorage(IOptions<LocalStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        Validate(file);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";

        var directoryPath = Path.Combine(_options.RootPath, folder);
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, fileName);

        await using var stream = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await file.CopyToAsync(stream, cancellationToken);

        return $"{_options.PublicBaseUrl}/{folder}/{fileName}";
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var physicalPath = relativePath
            .Replace(_options.PublicBaseUrl, _options.RootPath)
            .Replace('/', Path.DirectorySeparatorChar);

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }

        return Task.CompletedTask;
    }

    private void Validate(IFormFile file)
    {
        if (file is null || file.Length == 0)
            throw new InvalidOperationException("File is empty.");

        if (!_options.AllowedFileTypes.Contains(file.ContentType))
            throw new InvalidOperationException("Unsupported image format.");

        if (file.Length > _options.MaxFileSizeMB * 1024 * 1024)
            throw new InvalidOperationException("File size limit exceeded.");
    }
}
