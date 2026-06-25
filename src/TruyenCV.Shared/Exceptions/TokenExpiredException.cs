namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when the JWT access token or refresh token has expired (HTTP 401).
/// More specific than <see cref="UnauthorizedException"/> — allows the client
/// to distinguish between "never authenticated" and "token expired".
/// </summary>
public sealed class TokenExpiredException : Exception
{
    public TokenExpiredException()
        : base("Your session has expired. Please login again.") { }

    public TokenExpiredException(string message)
        : base(message) { }

    public TokenExpiredException(string tokenType)
        : base($"The {tokenType} has expired. Please authenticate again.") { }

    public TokenExpiredException(string message, Exception innerException)
        : base(message, innerException) { }
}
