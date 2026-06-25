using TruyenCV.Domain.Common;

namespace TruyenCV.Domain.Entities;

/// <summary>
/// Represents a system role (Admin, Moderator, User).
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // ── Navigation Properties ────────────────────────────────────────────────

    public ICollection<User> Users { get; set; } = [];
}
