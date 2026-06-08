using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class Author : AuditableEntityBase
{
    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public ICollection<BookEditionAuthor> BookEditionAuthors { get; set; } = new List<BookEditionAuthor>();
}
