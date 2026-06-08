using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(entity => entity.Id);

        builder.HasIndex(entity => new { entity.RoleId, entity.PermissionId })
            .IsUnique();

        builder.HasOne(entity => entity.Role)
            .WithMany(entity => entity.RolePermissions)
            .HasForeignKey(entity => entity.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.Permission)
            .WithMany(entity => entity.RolePermissions)
            .HasForeignKey(entity => entity.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
