using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class Publisher : AuditableEntityBase
{
    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public ICollection<BookEdition> BookEditions { get; set; } = new List<BookEdition>();
}
