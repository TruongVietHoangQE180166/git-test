namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when a requested operation is not supported or not implemented (HTTP 501).
/// </summary>
public sealed class NotImplementedException : Exception
{
    public NotImplementedException()
        : base("This feature is not yet implemented.") { }

    public NotImplementedException(string message)
        : base(message) { }

    public NotImplementedException(string featureName, string message)
        : base($"Feature '{featureName}' is not implemented: {message}") { }
}
