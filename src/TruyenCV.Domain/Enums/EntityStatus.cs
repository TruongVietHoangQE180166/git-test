namespace TruyenCV.Domain.Enums;

/// <summary>
/// Defines the lifecycle status of a domain entity.
/// Useful for soft-delete and active/inactive toggles.
/// </summary>
public enum EntityStatus
{
    /// <summary>The entity is active and visible.</summary>
    Active = 1,

    /// <summary>The entity is temporarily suspended or hidden.</summary>
    Inactive = 2,

    /// <summary>The entity is soft-deleted and should be ignored by standard queries.</summary>
    Deleted = 3
}
