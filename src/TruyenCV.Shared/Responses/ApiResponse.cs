namespace TruyenCV.Shared.Responses;

/// <summary>
/// Standard API response wrapper used for all endpoints.
/// </summary>
/// <typeparam name="T">The type of the response data.</typeparam>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    private ApiResponse() { }

    // ── Factory Methods ──────────────────────────────────────────────────────

    public static ApiResponse<T> Ok(T data, string message = "Request completed successfully.")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message, Data = default };
}

/// <summary>
/// Non-generic API response wrapper for endpoints that return no data.
/// </summary>
public sealed class ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;

    private ApiResponse() { }

    // ── Factory Methods ──────────────────────────────────────────────────────

    public static ApiResponse Ok(string message = "Request completed successfully.")
        => new() { Success = true, Message = message };

    public static ApiResponse Fail(string message)
        => new() { Success = false, Message = message };

    public static ApiResponse<T> Ok<T>(T data, string message = "Request completed successfully.")
        => ApiResponse<T>.Ok(data, message);

    public static ApiResponse<T> Fail<T>(string message)
        => ApiResponse<T>.Fail(message);
}
