namespace TruyenCV.Shared.Helpers;

/// <summary>
/// Utility methods for date and time operations.
/// </summary>
public static class DateTimeHelper
{
    private static readonly DateTimeOffset UnixEpoch =
        new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    /// <summary>Returns the current UTC time.</summary>
    public static DateTime UtcNow => DateTime.UtcNow;

    /// <summary>Returns the current UTC time as a <see cref="DateTimeOffset"/>.</summary>
    public static DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a Unix timestamp (seconds since epoch).
    /// </summary>
    public static long ToUnixTimestampSeconds(DateTime dateTime)
    {
        var utc = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        return (long)(utc - UnixEpoch.DateTime).TotalSeconds;
    }

    /// <summary>
    /// Converts a Unix timestamp (seconds) back to a UTC <see cref="DateTime"/>.
    /// </summary>
    public static DateTime FromUnixTimestampSeconds(long unixSeconds)
        => UnixEpoch.DateTime.AddSeconds(unixSeconds);

    /// <summary>
    /// Returns <c>true</c> if the given UTC <see cref="DateTime"/> is in the past.
    /// </summary>
    public static bool IsExpired(DateTime expiryUtc)
        => expiryUtc <= UtcNow;

    /// <summary>
    /// Returns <c>true</c> if the given UTC <see cref="DateTime"/> is still valid (in the future).
    /// </summary>
    public static bool IsValid(DateTime expiryUtc)
        => expiryUtc > UtcNow;

    /// <summary>
    /// Adds a number of minutes to the current UTC time.
    /// </summary>
    public static DateTime AddMinutesFromNow(int minutes)
        => UtcNow.AddMinutes(minutes);

    /// <summary>
    /// Adds a number of days to the current UTC time.
    /// </summary>
    public static DateTime AddDaysFromNow(int days)
        => UtcNow.AddDays(days);

    /// <summary>
    /// Formats a <see cref="DateTime"/> to a standard ISO 8601 string (UTC).
    /// </summary>
    public static string ToIso8601(DateTime dateTime)
        => dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
}
