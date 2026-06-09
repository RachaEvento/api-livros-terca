using MeuAcervo.Application.Models.Books;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Abstractions.Books;

public interface IBookCatalogRepository
{
    Task<BookEdition?> GetEditionByExternalReferenceAsync(string provider, string externalId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BookEdition>> GetEditionsByExternalReferencesAsync(
        IReadOnlyCollection<EditionExternalReferenceLookup> references,
        CancellationToken cancellationToken = default);

    Task<BookEdition?> GetEditionByIsbn13Async(string isbn13, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BookEdition>> GetEditionsByIsbn13Async(
        IReadOnlyCollection<string> isbn13Values,
        CancellationToken cancellationToken = default);

    Task<BookEdition?> GetEditionByIsbn10Async(string isbn10, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BookEdition>> GetEditionsByIsbn10Async(
        IReadOnlyCollection<string> isbn10Values,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BookEdition>> FindEditionsByNormalizedTitleAsync(string normalizedTitle, string? language, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BookEdition>> FindEditionsByNormalizedTitlesAsync(
        IReadOnlyCollection<string> normalizedTitles,
        CancellationToken cancellationToken = default);

    Task<BookWork?> GetWorkByExternalReferenceAsync(string provider, string externalId, CancellationToken cancellationToken = default);

    Task<BookWork?> FindWorkByNormalizedTitleAsync(string normalizedCanonicalTitle, string? primaryLanguage, CancellationToken cancellationToken = default);

    Task<Author?> FindAuthorByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);

    Task<Publisher?> FindPublisherByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);

    void AddAuthor(Author author);

    void AddPublisher(Publisher publisher);

    void AddBookWork(BookWork bookWork);

    void AddBookEdition(BookEdition bookEdition);

    void AddExternalBookReference(ExternalBookReference externalBookReference);

    void AddBookEditionAuthor(BookEditionAuthor bookEditionAuthor);
}
