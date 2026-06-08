using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class Role : TenantScopedAuditableEntityBase
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsSystemDefault { get; set; }

    public Tenant? Tenant { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
