using MeuAcervo.Domain.Common;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Domain.Entities;

public sealed class ExternalBookReference : AuditableEntityBase
{
    public string Provider { get; set; } = string.Empty;

    public string ExternalId { get; set; } = string.Empty;

    public ExternalBookReferenceType ReferenceType { get; set; }

    public Guid? BookWorkId { get; set; }

    public Guid? BookEditionId { get; set; }

    public string? ExternalUrl { get; set; }

    public BookWork? BookWork { get; set; }

    public BookEdition? BookEdition { get; set; }
}
