using TruyenCV.Domain.Common;

namespace TruyenCV.Domain.Entities;

/// <summary>
/// Represents an application user account.
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Whether the user has confirmed their email address.</summary>
    public bool IsEmailVerified { get; set; } = false;


    /// <summary>Foreign key to the Role table.</summary>
    public Guid RoleId { get; set; }

    // ── Navigation Properties ────────────────────────────────────────────────

    public Role Role { get; set; } = null!;

    public Profile? Profile { get; set; }

    public ICollection<AuthSession> AuthSessions { get; set; } = [];
}
