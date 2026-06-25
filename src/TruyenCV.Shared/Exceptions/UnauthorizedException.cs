namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when the request is not authenticated or the token is invalid/expired (HTTP 401).
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException()
        : base("You are not authorized. Please login to continue.") { }

    public UnauthorizedException(string message)
        : base(message) { }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException) { }
}
