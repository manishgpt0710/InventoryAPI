using Amazon.S3;
using Amazon.S3.Model;
using InventoryWebApi.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InventoryWebApi.Application.Services;

public class CloudflareR2Storage : IImageStorage
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;
    private readonly string _publicBaseUrl;
    private readonly string _allowedTypes;

    public CloudflareR2Storage(IConfiguration config)
    {
        var r2 = config.GetSection("R2");

        _bucket = r2["BucketName"]!;
        _publicBaseUrl = r2["PublicBaseUrl"]!;
        _allowedTypes = r2["AllowedFileTypes"]!;

        var endpoint = $"https://{r2["AccountId"]}.r2.cloudflarestorage.com";

        _s3 = new AmazonS3Client(
            r2["AccessKey"],
            r2["SecretKey"],
            new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true
            });
    }

    public async Task<string> UploadAsync(IFormFile file, string folder, CancellationToken cancellationToken)
    {
        ValidateImage(file);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{folder}/{Guid.NewGuid()}{extension}";

        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = fileName,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3.PutObjectAsync(request, cancellationToken);

        return $"{_publicBaseUrl}/{fileName}";
    }

    private void ValidateImage(IFormFile file)
    {
        if (!_allowedTypes.Contains(file.ContentType))
            throw new InvalidOperationException("Invalid image type");

        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("Image size exceeds 5MB");
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
