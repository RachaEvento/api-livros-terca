using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("loans");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.BorrowerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.BorrowerContact)
            .HasMaxLength(200);

        builder.Property(entity => entity.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(entity => entity.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(entity => new { entity.TenantId, entity.Status });

        builder.HasIndex(entity => new { entity.TenantId, entity.DueAtUtc });

        builder.HasIndex(entity => new { entity.TenantId, entity.UserLibraryItemId, entity.Status })
            .IsUnique()
            .HasFilter("\"Status\" = 'Active' AND \"ReturnedAtUtc\" IS NULL");

        builder.HasOne(entity => entity.UserLibraryItem)
            .WithMany(entity => entity.Loans)
            .HasForeignKey(entity => entity.UserLibraryItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
