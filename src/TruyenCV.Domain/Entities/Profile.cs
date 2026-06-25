using TruyenCV.Domain.Common;
using TruyenCV.Domain.Enums;

namespace TruyenCV.Domain.Entities;

/// <summary>
/// User's public-facing profile used for CV/portfolio display.
/// </summary>
public class Profile : BaseEntity
{
    /// <summary>FK → Users.Id (1-to-1).</summary>
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    /// <summary>URL-friendly slug: e.g. "nguyen-van-a".</summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>Whether the profile is publicly visible.</summary>
    public bool IsPublic { get; set; } = true;

    // ── Navigation Properties ────────────────────────────────────────────────

    public User User { get; set; } = null!;
}
