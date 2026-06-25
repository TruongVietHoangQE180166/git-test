namespace TruyenCV.Shared.Responses;

public class ApiResponse<T>
{
    public bool Succeeded { get; init; } = true;
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> Success(T data, string message = "Success")
        => new() { Data = data, Message = message };
}
