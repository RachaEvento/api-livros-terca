using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeuAcervo.Domain.Constants;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Infrastructure.Data.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Code)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(entity => entity.Description)
            .HasMaxLength(500);

        builder.HasIndex(entity => entity.Code)
            .IsUnique();

        var seedTimestamp = new DateTime(2026, 05, 25, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(PermissionCatalog.All.Select(permission => new Permission
        {
            Id = permission.Id,
            Code = permission.Code,
            Description = permission.Description,
            CreatedAtUtc = seedTimestamp,
            UpdatedAtUtc = seedTimestamp
        }));
    }
}
