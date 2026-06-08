using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class BookWorkConfiguration : IEntityTypeConfiguration<BookWork>
{
    public void Configure(EntityTypeBuilder<BookWork> builder)
    {
        builder.ToTable("book_works");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.CanonicalTitle)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(entity => entity.NormalizedCanonicalTitle)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(entity => entity.OriginalTitle)
            .HasMaxLength(500);

        builder.Property(entity => entity.Description)
            .HasMaxLength(8000);

        builder.Property(entity => entity.PrimaryLanguage)
            .HasMaxLength(16);

        builder.HasIndex(entity => entity.NormalizedCanonicalTitle);

        builder.HasIndex(entity => new { entity.NormalizedCanonicalTitle, entity.PrimaryLanguage });
    }
}
