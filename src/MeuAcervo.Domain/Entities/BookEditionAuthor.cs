using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class BookEditionAuthor : AuditableEntityBase
{
    public Guid BookEditionId { get; set; }

    public Guid AuthorId { get; set; }

    public int ContributionOrder { get; set; }

    public BookEdition? BookEdition { get; set; }

    public Author? Author { get; set; }
}
