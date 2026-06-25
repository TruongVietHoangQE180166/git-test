namespace TruyenCV.Shared.Constants;

/// <summary>
/// Centralised error message strings used across all layers.
/// Organised by domain / concern to keep them easy to find and maintain.
/// </summary>
public static class ErrorMessages
{
    // ═══════════════════════════════════════════════════════════════════════════
    // Generic / HTTP
    // ═══════════════════════════════════════════════════════════════════════════
    public const string NotFound            = "The requested resource was not found.";
    public const string BadRequest          = "The request is invalid or malformed.";
    public const string ValidationFailed    = "One or more validation errors occurred.";
    public const string Unauthorized        = "You are not authorized to perform this action.";
    public const string Forbidden           = "You do not have permission to access this resource.";
    public const string Conflict            = "A conflict occurred with the current state of the resource.";
    public const string TooManyRequests     = "Too many requests. Please slow down and try again later.";
    public const string RequestTimeout      = "The request timed out. Please try again.";
    public const string InternalServerError = "An unexpected error occurred. Please try again later.";
    public const string ServiceUnavailable  = "The service is temporarily unavailable. Please try again later.";
    public const string NotImplemented      = "This feature is not yet implemented.";
    public const string OperationFailed     = "The operation could not be completed.";
    public const string UnprocessableEntity = "The request could not be processed due to a business rule violation.";

    // ═══════════════════════════════════════════════════════════════════════════
    // Authentication & Session
    // ═══════════════════════════════════════════════════════════════════════════
    public const string InvalidCredentials      = "Email or password is incorrect.";
    public const string AccountNotFound         = "Account not found.";
    public const string AccountLocked           = "Your account has been locked. Please contact support.";
    public const string AccountDisabled         = "Your account has been disabled.";
    public const string AccountNotVerified      = "Your account is not yet verified. Please check your email.";
    public const string AccountAlreadyVerified  = "Your account is already verified.";
    public const string LoginRequired           = "You must be logged in to perform this action.";
    public const string SessionExpired          = "Your session has expired. Please login again.";
    public const string SessionNotFound         = "Session not found or already terminated.";
    public const string ConcurrentSession       = "A session is already active on another device.";

    // ═══════════════════════════════════════════════════════════════════════════
    // JWT / Token
    // ═══════════════════════════════════════════════════════════════════════════
    public const string TokenExpired            = "Your session has expired. Please login again.";
    public const string TokenInvalid            = "Invalid authentication token.";
    public const string TokenMalformed          = "The token is malformed and cannot be read.";
    public const string TokenSignatureInvalid   = "The token signature is invalid.";
    public const string RefreshTokenInvalid     = "Invalid or expired refresh token.";
    public const string RefreshTokenExpired     = "Your refresh token has expired. Please login again.";
    public const string RefreshTokenRevoked     = "Your refresh token has been revoked.";
    public const string RefreshTokenReused      = "Refresh token reuse detected. Session has been terminated for security.";
    public const string AccessTokenRequired     = "An access token is required.";

    // ═══════════════════════════════════════════════════════════════════════════
    // User
    // ═══════════════════════════════════════════════════════════════════════════
    public const string UserNotFound            = "User not found.";
    public const string EmailAlreadyExists      = "An account with this email already exists.";
    public const string UsernameAlreadyExists   = "This username is already taken.";
    public const string PasswordTooWeak         = "Password does not meet the minimum security requirements.";
    public const string PasswordMismatch        = "Passwords do not match.";
    public const string CurrentPasswordWrong    = "Current password is incorrect.";
    public const string SamePasswordNotAllowed  = "New password must be different from the current password.";
    public const string EmailNotConfirmed       = "Please verify your email address before proceeding.";
    public const string EmailUpdateSameValue    = "The new email is the same as the current email.";
    public const string UserDeleteSelf          = "You cannot delete your own account through this endpoint.";

    // ═══════════════════════════════════════════════════════════════════════════
    // Profile
    // ═══════════════════════════════════════════════════════════════════════════
    public const string ProfileNotFound         = "Profile not found.";
    public const string ProfileAlreadyExists    = "A profile already exists for this user.";
    public const string ProfileSlugTaken        = "This profile URL slug is already taken. Please choose a different one.";
    public const string ProfileNotOwned         = "You are not the owner of this profile.";

    // ═══════════════════════════════════════════════════════════════════════════
    // Role & Permission
    // ═══════════════════════════════════════════════════════════════════════════
    public const string RoleNotFound            = "Role not found.";
    public const string RoleAlreadyExists       = "A role with this name already exists.";
    public const string RoleAssignmentFailed    = "Failed to assign the role to the user.";
    public const string RoleRemovalFailed       = "Failed to remove the role from the user.";
    public const string CannotRemoveLastAdmin   = "Cannot remove the last admin from the system.";
    public const string InsufficientRole        = "Your role does not have sufficient permissions for this action.";

    // ═══════════════════════════════════════════════════════════════════════════
    // Email
    // ═══════════════════════════════════════════════════════════════════════════
    public const string EmailSendFailed         = "Failed to send the email. Please try again later.";
    public const string VerificationEmailSent   = "A verification email has been sent. Please check your inbox.";
    public const string PasswordResetEmailSent  = "A password reset link has been sent to your email.";
    public const string InvalidEmailFormat      = "The email address format is invalid.";
    public const string OtpInvalid              = "The OTP code is invalid or has expired.";
    public const string OtpExpired              = "The OTP code has expired. Please request a new one.";
    public const string OtpAlreadySent          = "A verification code was already sent. Please wait before requesting another.";

    // ═══════════════════════════════════════════════════════════════════════════
    // File / Upload
    // ═══════════════════════════════════════════════════════════════════════════
    public const string FileTooLarge            = "The uploaded file exceeds the maximum allowed size.";
    public const string FileTypeNotAllowed      = "This file type is not allowed. Please upload a valid file.";
    public const string FileEmpty               = "The uploaded file is empty.";
    public const string FileCorrupted           = "The uploaded file appears to be corrupted.";
    public const string FileNotFound            = "The requested file was not found.";
    public const string FileUploadFailed        = "The file upload failed. Please try again.";
    public const string TooManyFiles            = "Too many files uploaded at once. Please upload fewer files.";
    public const string ImageDimensionInvalid   = "Image dimensions exceed the allowed maximum.";

    // ═══════════════════════════════════════════════════════════════════════════
    // Database / Infrastructure
    // ═══════════════════════════════════════════════════════════════════════════
    public const string DatabaseError           = "A database error occurred. Please try again.";
    public const string DatabaseConnectionFailed = "Unable to connect to the database.";
    public const string ConcurrencyConflict     = "The record was modified by another process. Please refresh and try again.";
    public const string TransactionFailed       = "The database transaction failed and was rolled back.";

    // ═══════════════════════════════════════════════════════════════════════════
    // External Services
    // ═══════════════════════════════════════════════════════════════════════════
    public const string ExternalServiceUnavailable  = "An external service is currently unavailable. Please try again later.";
    public const string ExternalServiceTimeout      = "The external service did not respond in time.";
    public const string StorageServiceFailed        = "The file storage service is unavailable.";
    public const string CacheServiceFailed          = "The cache service is unavailable.";

    // ═══════════════════════════════════════════════════════════════════════════
    // Business Rules (CV / TruyenCV domain)
    // ═══════════════════════════════════════════════════════════════════════════
    public const string CvNotFound              = "CV not found.";
    public const string CvAlreadyPublished      = "This CV is already published.";
    public const string CvNotPublished          = "This CV has not been published yet.";
    public const string CvLimitReached          = "You have reached the maximum number of CVs allowed.";
    public const string CvNotOwned              = "You are not the owner of this CV.";
    public const string CvTemplateLocked        = "This template is locked and cannot be modified.";

    // ═══════════════════════════════════════════════════════════════════════════
    // Validation (field-level hints, for use in FluentValidation messages)
    // ═══════════════════════════════════════════════════════════════════════════
    public const string FieldRequired           = "'{0}' is required.";
    public const string FieldTooShort           = "'{0}' must be at least {1} characters.";
    public const string FieldTooLong            = "'{0}' must not exceed {1} characters.";
    public const string FieldInvalidFormat      = "'{0}' has an invalid format.";
    public const string FieldOutOfRange         = "'{0}' must be between {1} and {2}.";
    public const string FieldMustBePositive     = "'{0}' must be a positive number.";
    public const string FieldMustBeUnique       = "'{0}' must be unique.";
}
