using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class BookEditionConfiguration : IEntityTypeConfiguration<BookEdition>
{
    public void Configure(EntityTypeBuilder<BookEdition> builder)
    {
        builder.ToTable("book_editions");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Isbn10)
            .HasMaxLength(10);

        builder.Property(entity => entity.Isbn13)
            .HasMaxLength(13);

        builder.Property(entity => entity.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(entity => entity.NormalizedTitle)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(entity => entity.Subtitle)
            .HasMaxLength(500);

        builder.Property(entity => entity.Language)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(entity => entity.FormatDescriptor)
            .HasMaxLength(100);

        builder.Property(entity => entity.CoverImageUrl)
            .HasMaxLength(2048);

        builder.Property(entity => entity.EditionNumber)
            .HasMaxLength(50);

        builder.HasIndex(entity => entity.BookWorkId);

        builder.HasIndex(entity => entity.PublisherId);

        builder.HasIndex(entity => entity.Isbn10)
            .IsUnique()
            .HasFilter("\"Isbn10\" IS NOT NULL");

        builder.HasIndex(entity => entity.Isbn13)
            .IsUnique()
            .HasFilter("\"Isbn13\" IS NOT NULL");

        builder.HasIndex(entity => new { entity.NormalizedTitle, entity.Language });

        builder.HasOne(entity => entity.BookWork)
            .WithMany(entity => entity.BookEditions)
            .HasForeignKey(entity => entity.BookWorkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.Publisher)
            .WithMany(entity => entity.BookEditions)
            .HasForeignKey(entity => entity.PublisherId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
