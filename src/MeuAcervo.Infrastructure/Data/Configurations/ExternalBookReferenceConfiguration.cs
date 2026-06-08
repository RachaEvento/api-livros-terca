using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class ExternalBookReferenceConfiguration : IEntityTypeConfiguration<ExternalBookReference>
{
    public void Configure(EntityTypeBuilder<ExternalBookReference> builder)
    {
        builder.ToTable("external_book_references", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_external_book_references_target",
                "(\"ReferenceType\" = 'Work' AND \"BookWorkId\" IS NOT NULL AND \"BookEditionId\" IS NULL) OR " +
                "(\"ReferenceType\" = 'Edition' AND \"BookEditionId\" IS NOT NULL AND \"BookWorkId\" IS NULL)");
        });

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Provider)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.ExternalId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.ReferenceType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(entity => entity.ExternalUrl)
            .HasMaxLength(2048);

        builder.HasIndex(entity => new { entity.Provider, entity.ExternalId, entity.ReferenceType })
            .IsUnique();

        builder.HasIndex(entity => entity.BookWorkId);

        builder.HasIndex(entity => entity.BookEditionId);

        builder.HasOne(entity => entity.BookWork)
            .WithMany(entity => entity.ExternalBookReferences)
            .HasForeignKey(entity => entity.BookWorkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.BookEdition)
            .WithMany(entity => entity.ExternalBookReferences)
            .HasForeignKey(entity => entity.BookEditionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
