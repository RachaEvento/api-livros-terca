using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class CustomFieldOptionConfiguration : IEntityTypeConfiguration<CustomFieldOption>
{
    public void Configure(EntityTypeBuilder<CustomFieldOption> builder)
    {
        builder.ToTable("custom_field_options");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Value)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.Label)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(entity => new { entity.CustomFieldDefinitionId, entity.Value })
            .IsUnique();

        builder.HasOne(entity => entity.CustomFieldDefinition)
            .WithMany(entity => entity.Options)
            .HasForeignKey(entity => entity.CustomFieldDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
