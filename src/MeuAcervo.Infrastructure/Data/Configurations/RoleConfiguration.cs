using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.Description)
            .HasMaxLength(500);

        builder.HasIndex(entity => new { entity.TenantId, entity.Name })
            .IsUnique();

        builder.HasOne(entity => entity.Tenant)
            .WithMany(entity => entity.Roles)
            .HasForeignKey(entity => entity.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
