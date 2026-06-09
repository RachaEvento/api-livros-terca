using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class CustomFieldValueConfiguration : IEntityTypeConfiguration<CustomFieldValue>
{
    public void Configure(EntityTypeBuilder<CustomFieldValue> builder)
    {
        builder.ToTable("custom_field_values", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_custom_field_values_single_value",
                "(" +
                "CASE WHEN \"TextValue\" IS NOT NULL THEN 1 ELSE 0 END +" +
                " CASE WHEN \"NumberValue\" IS NOT NULL THEN 1 ELSE 0 END +" +
                " CASE WHEN \"DateValue\" IS NOT NULL THEN 1 ELSE 0 END +" +
                " CASE WHEN \"BooleanValue\" IS NOT NULL THEN 1 ELSE 0 END +" +
                " CASE WHEN \"OptionValue\" IS NOT NULL THEN 1 ELSE 0 END" +
                ") = 1");
        });

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.EntityType)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(entity => entity.TextValue)
            .HasMaxLength(4000);

        builder.Property(entity => entity.NumberValue)
            .HasPrecision(18, 4);

        builder.Property(entity => entity.OptionValue)
            .HasMaxLength(100);

        builder.HasIndex(entity => new { entity.TenantId, entity.EntityType, entity.EntityId });

        builder.HasIndex(entity => entity.CustomFieldDefinitionId);

        builder.HasIndex(entity => new { entity.TenantId, entity.EntityType, entity.EntityId, entity.CustomFieldDefinitionId })
            .IsUnique();

        builder.HasOne(entity => entity.CustomFieldDefinition)
            .WithMany(entity => entity.Values)
            .HasForeignKey(entity => entity.CustomFieldDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
