using MeuAcervo.Domain.Interfaces;

namespace MeuAcervo.Domain.Common;

public abstract class TenantScopedSoftDeletableAuditableEntityBase : TenantScopedAuditableEntityBase, ISoftDeletable
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedAtUtc { get; set; }
}
