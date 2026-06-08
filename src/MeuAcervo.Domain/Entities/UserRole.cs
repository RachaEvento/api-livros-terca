using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class UserRole : TenantScopedAuditableEntityBase
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public User? User { get; set; }

    public Role? Role { get; set; }
}
