namespace TruyenCV.Shared.Constants;

/// <summary>
/// Application-wide general constants.
/// </summary>
public static class AppConstants
{
    public const string AppName = "TruyenCV";
    public const string ApiVersion = "v1";

    // ── Pagination ───────────────────────────────────────────────────────────
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;
    public const int MinPageNumber = 1;

    // ── Token ────────────────────────────────────────────────────────────────
    public const int AccessTokenExpiryMinutes = 15;
    public const int RefreshTokenExpiryDays = 7;

    // ── Password ─────────────────────────────────────────────────────────────
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 128;

    // ── File Upload ──────────────────────────────────────────────────────────
    public const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    public static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
}
