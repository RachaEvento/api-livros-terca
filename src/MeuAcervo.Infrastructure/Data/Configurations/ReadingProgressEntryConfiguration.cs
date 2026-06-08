using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class ReadingProgressEntryConfiguration : IEntityTypeConfiguration<ReadingProgressEntry>
{
    public void Configure(EntityTypeBuilder<ReadingProgressEntry> builder)
    {
        builder.ToTable("reading_progress_entries");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.ProgressPercent)
            .HasPrecision(5, 2);

        builder.Property(entity => entity.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(entity => new { entity.TenantId, entity.UserLibraryItemId, entity.RecordedAtUtc });

        builder.HasOne(entity => entity.UserLibraryItem)
            .WithMany(entity => entity.ReadingProgressEntries)
            .HasForeignKey(entity => entity.UserLibraryItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
