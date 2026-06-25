using Microsoft.Extensions.Logging;
using TruyenCV.Application.Common.Interfaces;

namespace TruyenCV.Infrastructure.Services;

/// <summary>
/// Stub implementation for ICloudinaryService. 
/// Real implementation requires CloudinaryDotNet NuGet package.
/// </summary>
public class CloudinaryService : ICloudinaryService
{
    private readonly ILogger<CloudinaryService> _logger;

    public CloudinaryService(ILogger<CloudinaryService> logger)
    {
        _logger = logger;
    }

    public Task<CloudinaryUploadResult> UploadImageAsync(Stream stream, string fileName, string folder = "uploads", CancellationToken ct = default)
    {
        _logger.LogWarning("CloudinaryService.UploadImageAsync called but it is a stub. Simulating successful upload for {FileName}", fileName);
        
        // Simulate upload
        var fakeResult = new CloudinaryUploadResult(
            PublicId: $"{folder}/fake_{Guid.NewGuid()}",
            SecureUrl: $"https://res.cloudinary.com/demo/image/upload/v1234567890/{folder}/fake_{Guid.NewGuid()}.jpg",
            Format: "jpg",
            Bytes: stream.Length > 0 ? stream.Length : 1024
        );

        return Task.FromResult(fakeResult);
    }

    public Task<bool> DeleteImageAsync(string publicId, CancellationToken ct = default)
    {
        _logger.LogWarning("CloudinaryService.DeleteImageAsync called but it is a stub. Simulating successful deletion for {PublicId}", publicId);
        return Task.FromResult(true);
    }
}
