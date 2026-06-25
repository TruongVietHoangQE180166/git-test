namespace TruyenCV.Shared.Results;

/// <summary>
/// Represents the outcome of an operation without a return value.
/// Use <see cref="Result{T}"/> when you need to return data.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("A successful result cannot have an error.");
        if (!isSuccess && error is null)
            throw new InvalidOperationException("A failed result must have an error message.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    // ── Factory Methods ──────────────────────────────────────────────────────

    public static Result Success() => new(true, null);

    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);

    // ── Implicit Conversion ──────────────────────────────────────────────────

    public static implicit operator Result(string error) => Failure(error);
}
