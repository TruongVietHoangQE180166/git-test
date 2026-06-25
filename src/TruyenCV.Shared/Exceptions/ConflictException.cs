namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when a resource conflict occurs (HTTP 409).
/// Examples: duplicate email, username already taken, slug collision.
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException()
        : base("A conflict occurred with the current state of the resource.") { }

    public ConflictException(string message)
        : base(message) { }

    public ConflictException(string resourceName, string fieldName, object value)
        : base($"'{resourceName}' with {fieldName} '{value}' already exists.") { }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException) { }
}
