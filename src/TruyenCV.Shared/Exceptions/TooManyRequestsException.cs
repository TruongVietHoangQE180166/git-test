namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when the client exceeds the allowed request rate (HTTP 429 Too Many Requests).
/// </summary>
public sealed class TooManyRequestsException : Exception
{
    /// <summary>Number of seconds the client should wait before retrying.</summary>
    public int? RetryAfterSeconds { get; }

    public TooManyRequestsException()
        : base("Too many requests. Please slow down and try again later.") { }

    public TooManyRequestsException(string message)
        : base(message) { }

    public TooManyRequestsException(int retryAfterSeconds)
        : base($"Too many requests. Please try again after {retryAfterSeconds} seconds.")
    {
        RetryAfterSeconds = retryAfterSeconds;
    }

    public TooManyRequestsException(string message, Exception innerException)
        : base(message, innerException) { }
}
