using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruyenCV.Domain.Entities;
using TruyenCV.Domain.Enums;

namespace TruyenCV.Infrastructure.Persistence.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnType("char(36)");

        builder.Property(p => p.UserId)
            .HasColumnType("char(36)");

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Bio)
            .HasMaxLength(1000);

        builder.Property(p => p.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(p => p.DateOfBirth)
            .HasColumnType("date");

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.Property(p => p.IsPublic)
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasColumnType("datetime(6)");

        builder.Property(p => p.UpdatedAt)
            .HasColumnType("datetime(6)");
    }
}
