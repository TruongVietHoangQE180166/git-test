using System;

namespace TruyenCV.Shared.Exceptions;

public static class ExceptionExtensions
{
    /// <summary>
    /// Checks if an exception represents a critical system/infrastructure failure
    /// (e.g. database down, SMTP down, bugs, null references, timeouts) rather than
    /// a normal business/validation error (e.g. invalid password, user validation failure).
    /// </summary>
    public static bool IsSystemError(this Exception exception)
    {
        return exception is not (
            ValidationException or
            BadRequestException or
            NotFoundException or
            ConflictException or
            UnauthorizedException or
            ForbiddenException or
            BusinessRuleException or
            TokenExpiredException or
            TooManyRequestsException or
            FileUploadException
        );
    }
}
