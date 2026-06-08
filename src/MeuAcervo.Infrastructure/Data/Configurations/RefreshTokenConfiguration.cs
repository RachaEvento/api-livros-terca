using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.TokenHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(entity => entity.JwtId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(entity => entity.CreatedByIp)
            .HasMaxLength(64);

        builder.Property(entity => entity.ReplacedByTokenHash)
            .HasMaxLength(512);

        builder.HasIndex(entity => entity.TokenHash)
            .IsUnique();

        builder.HasIndex(entity => entity.TenantId);

        builder.HasOne(entity => entity.User)
            .WithMany(entity => entity.RefreshTokens)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
