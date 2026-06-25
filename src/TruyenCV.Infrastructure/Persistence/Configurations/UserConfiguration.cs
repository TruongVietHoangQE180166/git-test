using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruyenCV.Domain.Entities;
using TruyenCV.Domain.Enums;

namespace TruyenCV.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnType("char(36)");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.IsEmailVerified)
            .HasDefaultValue(false);

        builder.Property(u => u.Status)
            .HasConversion<int>()
            .HasDefaultValue(EntityStatus.Active);

        builder.Property(u => u.RoleId)
            .HasColumnType("char(36)");

        builder.Property(u => u.CreatedAt)
            .HasColumnType("datetime(6)");

        builder.Property(u => u.UpdatedAt)
            .HasColumnType("datetime(6)");

        // ── Relationships ────────────────────────────────────────────────────

        // Many Users → One Role
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // One User → One Profile (optional)
        builder.HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One User → Many AuthSessions
        builder.HasMany(u => u.AuthSessions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
