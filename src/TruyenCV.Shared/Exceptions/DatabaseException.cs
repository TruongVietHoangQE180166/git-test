namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when a database operation fails unexpectedly (HTTP 500).
/// Should be caught at the infrastructure layer and wrapped with context before re-throwing.
/// </summary>
public sealed class DatabaseException : Exception
{
    /// <summary>The operation that was being attempted (e.g. "Insert", "Update", "Query").</summary>
    public string? Operation { get; }

    public DatabaseException()
        : base("A database error occurred.") { }

    public DatabaseException(string message)
        : base(message) { }

    public DatabaseException(string message, string operation)
        : base(message)
    {
        Operation = operation;
    }

    public DatabaseException(string message, Exception innerException)
        : base(message, innerException) { }

    public DatabaseException(string message, string operation, Exception innerException)
        : base(message, innerException)
    {
        Operation = operation;
    }
}
