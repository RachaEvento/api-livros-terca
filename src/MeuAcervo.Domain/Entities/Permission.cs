using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class Permission : AuditableEntityBase
{
    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
