using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class RolePermission : AuditableEntityBase
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public Role? Role { get; set; }

    public Permission? Permission { get; set; }
}
