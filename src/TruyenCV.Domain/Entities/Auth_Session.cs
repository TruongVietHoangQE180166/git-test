using TruyenCV.Domain.Common;

namespace TruyenCV.Domain.Entities;

/// <summary>
/// Tracks active refresh-token sessions for a user.
/// One user can have multiple sessions (different devices).
/// </summary>
public class AuthSession : BaseEntity
{
    /// <summary>FK → Users.Id.</summary>
    public Guid UserId { get; set; }

    /// <summary>Hashed or raw refresh token stored for validation.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>User-Agent / browser fingerprint (optional).</summary>
    public string? DeviceInfo { get; set; }

    /// <summary>IP address of the client that created the session.</summary>
    public string? IpAddress { get; set; }

    /// <summary>Whether this session has been explicitly revoked.</summary>
    public bool IsRevoked { get; set; } = false;

    /// <summary>UTC time when the refresh token expires.</summary>
    public DateTime ExpiresAt { get; set; }

    // ── Navigation Properties ────────────────────────────────────────────────

    public User User { get; set; } = null!;
}
