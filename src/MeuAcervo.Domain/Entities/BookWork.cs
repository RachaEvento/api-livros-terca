using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class BookWork : AuditableEntityBase
{
    public string CanonicalTitle { get; set; } = string.Empty;

    public string NormalizedCanonicalTitle { get; set; } = string.Empty;

    public string? OriginalTitle { get; set; }

    public string? Description { get; set; }

    public int? FirstPublicationYear { get; set; }

    public string? PrimaryLanguage { get; set; }

    public ICollection<BookEdition> BookEditions { get; set; } = new List<BookEdition>();

    public ICollection<ExternalBookReference> ExternalBookReferences { get; set; } = new List<ExternalBookReference>();
}
