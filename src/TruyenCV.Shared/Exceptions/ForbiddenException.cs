namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when the authenticated user does not have permission to perform the action (HTTP 403).
/// </summary>
public sealed class ForbiddenException : Exception
{
    public ForbiddenException()
        : base("You do not have permission to perform this action.") { }

    public ForbiddenException(string message)
        : base(message) { }

    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException) { }
}
