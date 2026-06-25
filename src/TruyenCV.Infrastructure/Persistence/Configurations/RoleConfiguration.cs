using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruyenCV.Domain.Entities;
using TruyenCV.Shared.Constants;

namespace TruyenCV.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnType("char(36)");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.Property(r => r.CreatedAt)
            .HasColumnType("datetime(6)");

        builder.Property(r => r.UpdatedAt)
            .HasColumnType("datetime(6)");

        // ── Seed Data ────────────────────────────────────────────────────────
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new Role
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = RoleConstants.Admin,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Role
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = RoleConstants.Moderator,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Role
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = RoleConstants.User,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }
}
