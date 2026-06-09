using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class CustomFieldDefinitionConfiguration : IEntityTypeConfiguration<CustomFieldDefinition>
{
    public void Configure(EntityTypeBuilder<CustomFieldDefinition> builder)
    {
        builder.ToTable("custom_field_definitions");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.EntityType)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(entity => entity.Label)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(entity => entity.DataType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(entity => new { entity.TenantId, entity.EntityType, entity.SortOrder });
    }
}
