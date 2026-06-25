using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruyenCV.Domain.Entities;

namespace TruyenCV.Infrastructure.Persistence.Configurations;

public class AuthConfiguration : IEntityTypeConfiguration<AuthSession>
{
    public void Configure(EntityTypeBuilder<AuthSession> builder)
    {
        builder.ToTable("auth_sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnType("char(36)");

        builder.Property(s => s.UserId)
            .HasColumnType("char(36)");

        builder.Property(s => s.RefreshToken)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasIndex(s => s.RefreshToken)
            .IsUnique();

        builder.HasIndex(s => s.UserId);

        builder.Property(s => s.DeviceInfo)
            .HasMaxLength(500);

        builder.Property(s => s.IpAddress)
            .HasMaxLength(45); // Supports IPv6

        builder.Property(s => s.IsRevoked)
            .HasDefaultValue(false);

        builder.Property(s => s.ExpiresAt)
            .HasColumnType("datetime(6)");

        builder.Property(s => s.CreatedAt)
            .HasColumnType("datetime(6)");

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("datetime(6)");
    }
}
