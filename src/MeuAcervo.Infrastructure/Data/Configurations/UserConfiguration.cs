using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(entity => entity.NormalizedEmail)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(entity => entity.Username)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(entity => entity.NormalizedUsername)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(entity => entity.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(entity => entity.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.AvatarUrl)
            .HasMaxLength(2048);

        builder.Property(entity => entity.Bio)
            .HasMaxLength(2000);

        builder.HasIndex(entity => entity.NormalizedEmail)
            .IsUnique();

        builder.HasIndex(entity => entity.NormalizedUsername)
            .IsUnique();

        builder.HasIndex(entity => entity.TenantId);

        builder.HasOne(entity => entity.Tenant)
            .WithMany(entity => entity.Users)
            .HasForeignKey(entity => entity.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity => entity.UserProfile)
            .WithOne(entity => entity.User)
            .HasForeignKey<UserProfile>(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
