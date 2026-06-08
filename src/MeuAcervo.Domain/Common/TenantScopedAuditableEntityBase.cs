using MeuAcervo.Domain.Interfaces;

namespace MeuAcervo.Domain.Common;

public abstract class TenantScopedAuditableEntityBase : AuditableEntityBase, ITenantScoped
{
    public Guid TenantId { get; set; }
}
