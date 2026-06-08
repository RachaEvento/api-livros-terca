using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(entity => entity.Id);

        builder.HasIndex(entity => new { entity.TenantId, entity.UserId, entity.RoleId })
            .IsUnique();

        builder.HasOne(entity => entity.User)
            .WithMany(entity => entity.UserRoles)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.Role)
            .WithMany(entity => entity.UserRoles)
            .HasForeignKey(entity => entity.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
