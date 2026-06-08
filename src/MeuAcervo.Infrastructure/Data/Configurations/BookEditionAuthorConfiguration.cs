using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class BookEditionAuthorConfiguration : IEntityTypeConfiguration<BookEditionAuthor>
{
    public void Configure(EntityTypeBuilder<BookEditionAuthor> builder)
    {
        builder.ToTable("book_edition_authors");

        builder.HasKey(entity => entity.Id);

        builder.HasIndex(entity => new { entity.BookEditionId, entity.AuthorId })
            .IsUnique();

        builder.HasIndex(entity => new { entity.BookEditionId, entity.ContributionOrder });

        builder.HasOne(entity => entity.BookEdition)
            .WithMany(entity => entity.BookEditionAuthors)
            .HasForeignKey(entity => entity.BookEditionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.Author)
            .WithMany(entity => entity.BookEditionAuthors)
            .HasForeignKey(entity => entity.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
