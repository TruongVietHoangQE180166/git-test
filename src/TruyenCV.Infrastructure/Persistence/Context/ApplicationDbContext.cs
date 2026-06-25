using Microsoft.EntityFrameworkCore;
using TruyenCV.Domain.Common;
using TruyenCV.Domain.Entities;

namespace TruyenCV.Infrastructure.Persistence.Context;

/// <summary>
/// Main EF Core database context for TruyenCV using MySQL (Pomelo).
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── DbSets ───────────────────────────────────────────────────────────────

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();

    // ── Model Configuration ──────────────────────────────────────────────────

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> implementations in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    // ── Audit Timestamps ─────────────────────────────────────────────────────

    /// <summary>
    /// Automatically updates <see cref="BaseEntity.UpdatedAt"/> before saving changes.
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = utcNow;

            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = utcNow;
        }
    }
}
