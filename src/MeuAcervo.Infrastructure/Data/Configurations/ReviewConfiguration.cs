using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Title)
            .HasMaxLength(200);

        builder.Property(entity => entity.Content)
            .HasMaxLength(8000)
            .IsRequired();

        builder.Property(entity => entity.Visibility)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(entity => new { entity.TenantId, entity.UserId, entity.UserLibraryItemId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = FALSE");

        builder.HasIndex(entity => new { entity.TenantId, entity.Visibility });

        builder.HasOne(entity => entity.User)
            .WithMany(entity => entity.Reviews)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity => entity.UserLibraryItem)
            .WithOne(entity => entity.Review)
            .HasForeignKey<Review>(entity => entity.UserLibraryItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
