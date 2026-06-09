using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.Slug)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(entity => entity.Color)
            .HasMaxLength(32);

        builder.Property(entity => entity.Description)
            .HasMaxLength(500);

        builder.HasIndex(entity => new { entity.TenantId, entity.Slug })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = FALSE");
    }
}
