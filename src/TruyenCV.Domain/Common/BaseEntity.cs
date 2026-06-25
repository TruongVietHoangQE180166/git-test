namespace TruyenCV.Domain.Common;

/// <summary>
/// Abstract base class for all domain entities.
/// Provides common audit fields: Id, CreatedAt, UpdatedAt.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Lifecycle status of the entity. Used for soft-delete instead of hard-delete.
    /// </summary>
    public Enums.EntityStatus Status { get; set; } = Enums.EntityStatus.Active;
}
