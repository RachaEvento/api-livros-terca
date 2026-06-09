using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.FavoriteQuoteOrHeadline)
            .HasMaxLength(280);

        builder.HasIndex(entity => entity.UserId)
            .IsUnique();

        builder.HasIndex(entity => new { entity.TenantId, entity.IsPublicProfileEnabled });

        builder.HasOne(entity => entity.User)
            .WithOne(entity => entity.UserProfile)
            .HasForeignKey<UserProfile>(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
