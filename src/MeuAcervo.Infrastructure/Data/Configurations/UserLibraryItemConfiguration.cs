using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class UserLibraryItemConfiguration : IEntityTypeConfiguration<UserLibraryItem>
{
    public void Configure(EntityTypeBuilder<UserLibraryItem> builder)
    {
        builder.ToTable("user_library_items");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.ShelfType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(entity => entity.ReadingStatus)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(entity => entity.AcquisitionFormat)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(entity => entity.OwnershipType)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(entity => entity.ProgressPercent)
            .HasPrecision(5, 2);

        builder.Property(entity => entity.PhysicalLocation)
            .HasMaxLength(256);

        builder.Property(entity => entity.Condition)
            .HasMaxLength(100);

        builder.Property(entity => entity.PrivateNotes)
            .HasMaxLength(4000);

        builder.HasIndex(entity => new { entity.TenantId, entity.UserId, entity.ShelfType });

        builder.HasIndex(entity => new { entity.TenantId, entity.UserId, entity.ReadingStatus });

        builder.HasIndex(entity => new { entity.TenantId, entity.UserId, entity.IsFavorite });

        builder.HasIndex(entity => new { entity.TenantId, entity.UserId, entity.StartedAt });

        builder.HasIndex(entity => new { entity.TenantId, entity.UserId, entity.FinishedAt });

        builder.HasIndex(entity => entity.BookEditionId);

        builder.HasIndex(entity => entity.UpdatedAtUtc);

        builder.HasIndex(entity => new { entity.TenantId, entity.UserId, entity.BookEditionId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = FALSE");

        builder.HasOne(entity => entity.User)
            .WithMany(entity => entity.UserLibraryItems)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity => entity.BookEdition)
            .WithMany(entity => entity.UserLibraryItems)
            .HasForeignKey(entity => entity.BookEditionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
