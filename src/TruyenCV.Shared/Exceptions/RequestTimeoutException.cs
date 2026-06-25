namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when a request times out (HTTP 408 Request Timeout).
/// Typically used for long-running operations that exceed the allowed duration.
/// </summary>
public sealed class RequestTimeoutException : Exception
{
    /// <summary>The timeout duration in seconds, if known.</summary>
    public int? TimeoutSeconds { get; }

    public RequestTimeoutException()
        : base("The request timed out. Please try again.") { }

    public RequestTimeoutException(string message)
        : base(message) { }

    public RequestTimeoutException(int timeoutSeconds)
        : base($"The request exceeded the allowed timeout of {timeoutSeconds} seconds.")
    {
        TimeoutSeconds = timeoutSeconds;
    }

    public RequestTimeoutException(string message, Exception innerException)
        : base(message, innerException) { }
}
