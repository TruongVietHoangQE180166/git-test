namespace TruyenCV.Application.Common.Interfaces;

/// <summary>
/// Contract for cloud image storage operations (Cloudinary).
/// </summary>
public interface ICloudinaryService
{
    /// <summary>
    /// Uploads an image from a stream and returns the secure URL.
    /// </summary>
    /// <param name="stream">The image data stream.</param>
    /// <param name="fileName">Original file name (used for public_id generation).</param>
    /// <param name="folder">Target Cloudinary folder (e.g. "avatars", "cv-photos").</param>
    Task<CloudinaryUploadResult> UploadImageAsync(
        Stream stream,
        string fileName,
        string folder = "uploads",
        CancellationToken ct = default);

    /// <summary>Deletes an image by its Cloudinary public_id.</summary>
    Task<bool> DeleteImageAsync(string publicId, CancellationToken ct = default);
}

/// <summary>
/// Result returned by a successful Cloudinary upload.
/// </summary>
public sealed record CloudinaryUploadResult(
    string PublicId,
    string SecureUrl,
    string Format,
    long Bytes);
