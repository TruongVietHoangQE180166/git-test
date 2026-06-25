namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when the request is syntactically valid but semantically incorrect (HTTP 400).
/// Use for general bad input that is not a validation rule violation.
/// For field-level validation errors, use <see cref="ValidationException"/> instead.
/// </summary>
public sealed class BadRequestException : Exception
{
    public BadRequestException()
        : base("The request is invalid or malformed.") { }

    public BadRequestException(string message)
        : base(message) { }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException) { }
}
