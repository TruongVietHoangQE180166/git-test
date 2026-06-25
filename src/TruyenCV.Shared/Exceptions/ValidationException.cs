namespace TruyenCV.Shared.Exceptions;

/// <summary>
/// Thrown when one or more validation rules are violated (HTTP 400).
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation errors, keyed by field name.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string field, string error)
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    public ValidationException(IEnumerable<ValidationError> validationErrors)
        : base("One or more validation failures have occurred.")
    {
        Errors = validationErrors
            .GroupBy(e => e.Field)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Message).ToArray());
    }
}

/// <summary>
/// Represents a single validation error for a specific field.
/// </summary>
public sealed record ValidationError(string Field, string Message);
