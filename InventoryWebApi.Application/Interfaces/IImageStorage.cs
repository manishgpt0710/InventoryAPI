using Microsoft.AspNetCore.Http;

namespace InventoryWebApi.Application.Interfaces;

public interface IImageStorage
{
    Task<string> UploadAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
    Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
}

