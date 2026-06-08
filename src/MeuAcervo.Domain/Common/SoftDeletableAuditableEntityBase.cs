using MeuAcervo.Domain.Interfaces;

namespace MeuAcervo.Domain.Common;

public abstract class SoftDeletableAuditableEntityBase : AuditableEntityBase, ISoftDeletable
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedAtUtc { get; set; }
}
