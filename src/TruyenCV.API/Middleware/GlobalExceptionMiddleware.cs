using System.Net;
using System.Text.Json;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Responses;

namespace TruyenCV.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (ex.IsSystemError())
            {
                _logger.LogError(ex, "An unhandled system exception occurred during the request.");
            }
            else
            {
                _logger.LogInformation("Request failed due to business exception: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
            }
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        int statusCode;
        string message;
        IReadOnlyDictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case ValidationException e:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = e.Message;
                errors = e.Errors;
                break;
            case BadRequestException e:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = e.Message;
                break;
            case FileUploadException e:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = e.Message;
                break;
            case UnauthorizedException e:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = e.Message;
                break;
            case TokenExpiredException e:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = e.Message;
                break;
            case ForbiddenException e:
                statusCode = (int)HttpStatusCode.Forbidden;
                message = e.Message;
                break;
            case NotFoundException e:
                statusCode = (int)HttpStatusCode.NotFound;
                message = e.Message;
                break;
            case ConflictException e:
                statusCode = (int)HttpStatusCode.Conflict;
                message = e.Message;
                break;
            case RequestTimeoutException e:
                statusCode = (int)HttpStatusCode.RequestTimeout;
                message = e.Message;
                break;
            case TooManyRequestsException e:
                statusCode = StatusCodes.Status429TooManyRequests;
                message = e.Message;
                if (e.RetryAfterSeconds.HasValue)
                {
                    context.Response.Headers["Retry-After"] = e.RetryAfterSeconds.Value.ToString();
                }
                break;
            case BusinessRuleException e:
                statusCode = (int)HttpStatusCode.UnprocessableEntity;
                message = e.Message;
                break;
            case TruyenCV.Shared.Exceptions.NotImplementedException e:
                statusCode = (int)HttpStatusCode.NotImplemented;
                message = e.Message;
                break;
            case System.NotImplementedException e:
                statusCode = (int)HttpStatusCode.NotImplemented;
                message = e.Message;
                break;
            case ExternalServiceException e:
                statusCode = (int)HttpStatusCode.BadGateway;
                message = e.Message;
                break;
            case ServiceUnavailableException e:
                statusCode = (int)HttpStatusCode.ServiceUnavailable;
                message = e.Message;
                if (e.RetryAfterSeconds.HasValue)
                {
                    context.Response.Headers["Retry-After"] = e.RetryAfterSeconds.Value.ToString();
                }
                break;
            case DatabaseException e:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = e.Message;
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        context.Response.StatusCode = statusCode;
        var response = ErrorResponse.Create(statusCode, message, context.TraceIdentifier, errors);

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return context.Response.WriteAsync(result);
    }
}
