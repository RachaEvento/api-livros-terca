using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class BookEdition : AuditableEntityBase
{
    public Guid BookWorkId { get; set; }

    public string? Isbn10 { get; set; }

    public string? Isbn13 { get; set; }

    public string Title { get; set; } = string.Empty;

    public string NormalizedTitle { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    public Guid? PublisherId { get; set; }

    public DateTime? PublishedAt { get; set; }

    public int? PageCount { get; set; }

    public string Language { get; set; } = string.Empty;

    public string? FormatDescriptor { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? EditionNumber { get; set; }

    public BookWork? BookWork { get; set; }

    public Publisher? Publisher { get; set; }

    public ICollection<BookEditionAuthor> BookEditionAuthors { get; set; } = new List<BookEditionAuthor>();

    public ICollection<ExternalBookReference> ExternalBookReferences { get; set; } = new List<ExternalBookReference>();

    public ICollection<UserLibraryItem> UserLibraryItems { get; set; } = new List<UserLibraryItem>();
}
