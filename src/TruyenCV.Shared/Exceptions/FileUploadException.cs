namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when a file upload operation fails (HTTP 400).
/// Examples: file too large, unsupported format, corrupted file.
/// </summary>
public sealed class FileUploadException : Exception
{
    /// <summary>The name of the file that caused the error, if available.</summary>
    public string? FileName { get; }

    public FileUploadException()
        : base("The file upload failed.") { }

    public FileUploadException(string message)
        : base(message) { }

    public FileUploadException(string message, string fileName)
        : base(message)
    {
        FileName = fileName;
    }

    public FileUploadException(string message, Exception innerException)
        : base(message, innerException) { }
}
