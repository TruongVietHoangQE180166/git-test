namespace TruyenCV.Shared.Responses;

/// <summary>
/// Structured error response returned by the global exception middleware.
/// </summary>
public sealed class ErrorResponse
{
    /// <summary>HTTP status code (e.g. 400, 401, 403, 404, 500).</summary>
    public int StatusCode { get; init; }

    /// <summary>Human-readable summary of the error.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Field-level validation errors, keyed by field name.
    /// Only populated for 400 validation failures.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    /// <summary>Correlation / trace identifier for log lookup.</summary>
    public string? TraceId { get; init; }

    /// <summary>UTC timestamp of the error.</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    // ── Factory Methods ──────────────────────────────────────────────────────

    public static ErrorResponse Create(
        int statusCode,
        string message,
        string? traceId = null,
        IReadOnlyDictionary<string, string[]>? errors = null)
        => new()
        {
            StatusCode = statusCode,
            Message = message,
            TraceId = traceId,
            Errors = errors
        };
}
