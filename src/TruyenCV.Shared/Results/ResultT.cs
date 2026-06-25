namespace TruyenCV.Shared.Results;

/// <summary>
/// Represents the outcome of an operation that returns a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the returned value on success.</typeparam>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(bool isSuccess, T? value, string? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value. Throws <see cref="InvalidOperationException"/> if the result is a failure.
    /// </summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    // ── Factory Methods ──────────────────────────────────────────────────────

    public static Result<T> Success(T value) => new(true, value, null);

    public new static Result<T> Failure(string error) => new(false, default, error);

    // ── Implicit Conversions ─────────────────────────────────────────────────

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(string error) => Failure(error);
}
