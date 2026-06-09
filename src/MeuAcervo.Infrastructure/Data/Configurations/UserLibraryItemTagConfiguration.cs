using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class UserLibraryItemTagConfiguration : IEntityTypeConfiguration<UserLibraryItemTag>
{
    public void Configure(EntityTypeBuilder<UserLibraryItemTag> builder)
    {
        builder.ToTable("user_library_item_tags");

        builder.HasKey(entity => entity.Id);

        builder.HasIndex(entity => new { entity.TenantId, entity.UserLibraryItemId, entity.TagId })
            .IsUnique();

        builder.HasOne(entity => entity.UserLibraryItem)
            .WithMany(entity => entity.UserLibraryItemTags)
            .HasForeignKey(entity => entity.UserLibraryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.Tag)
            .WithMany(entity => entity.UserLibraryItemTags)
            .HasForeignKey(entity => entity.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
