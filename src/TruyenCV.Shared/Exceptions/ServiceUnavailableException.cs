namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when the service is temporarily unavailable (HTTP 503 Service Unavailable).
/// Examples: maintenance mode, database unreachable, external dependency down.
/// </summary>
public sealed class ServiceUnavailableException : Exception
{
    /// <summary>Estimated time (in seconds) until the service is expected to recover.</summary>
    public int? RetryAfterSeconds { get; }

    public ServiceUnavailableException()
        : base("The service is temporarily unavailable. Please try again later.") { }

    public ServiceUnavailableException(string message)
        : base(message) { }

    public ServiceUnavailableException(string message, int retryAfterSeconds)
        : base(message)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }

    public ServiceUnavailableException(string message, Exception innerException)
        : base(message, innerException) { }
}
