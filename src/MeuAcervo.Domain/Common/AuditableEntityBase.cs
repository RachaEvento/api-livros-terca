namespace MeuAcervo.Domain.Common;

public abstract class AuditableEntityBase : EntityBase
{
    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
