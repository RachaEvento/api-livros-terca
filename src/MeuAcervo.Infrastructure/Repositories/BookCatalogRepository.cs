using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Infrastructure.Data;

namespace MeuAcervo.Infrastructure.Repositories;

public sealed class BookCatalogRepository : IBookCatalogRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BookCatalogRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BookEdition?> GetEditionByExternalReferenceAsync(string provider, string externalId, CancellationToken cancellationToken = default)
    {
        return BookEditionGraph()
            .FirstOrDefaultAsync(
                edition => edition.ExternalBookReferences.Any(reference =>
                    reference.ReferenceType == Domain.Enums.ExternalBookReferenceType.Edition
                    && reference.Provider == provider
                    && reference.ExternalId == externalId),
                cancellationToken);
    }

    public Task<BookEdition?> GetEditionByIsbn13Async(string isbn13, CancellationToken cancellationToken = default)
    {
        return BookEditionGraph()
            .FirstOrDefaultAsync(edition => edition.Isbn13 == isbn13, cancellationToken);
    }

    public Task<BookEdition?> GetEditionByIsbn10Async(string isbn10, CancellationToken cancellationToken = default)
    {
        return BookEditionGraph()
            .FirstOrDefaultAsync(edition => edition.Isbn10 == isbn10, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BookEdition>> FindEditionsByNormalizedTitleAsync(string normalizedTitle, string? language, CancellationToken cancellationToken = default)
    {
        var query = BookEditionGraph()
            .Where(edition => edition.NormalizedTitle == normalizedTitle);

        if (!string.IsNullOrWhiteSpace(language))
        {
            query = query.Where(edition => edition.Language == language);
        }

        return await query.ToArrayAsync(cancellationToken);
    }

    public Task<BookWork?> GetWorkByExternalReferenceAsync(string provider, string externalId, CancellationToken cancellationToken = default)
    {
        return BookWorkGraph()
            .FirstOrDefaultAsync(
                work => work.ExternalBookReferences.Any(reference =>
                    reference.ReferenceType == Domain.Enums.ExternalBookReferenceType.Work
                    && reference.Provider == provider
                    && reference.ExternalId == externalId),
                cancellationToken);
    }

    public Task<BookWork?> FindWorkByNormalizedTitleAsync(string normalizedCanonicalTitle, string? primaryLanguage, CancellationToken cancellationToken = default)
    {
        var query = BookWorkGraph()
            .Where(work => work.NormalizedCanonicalTitle == normalizedCanonicalTitle);

        if (!string.IsNullOrWhiteSpace(primaryLanguage))
        {
            query = query.Where(work => work.PrimaryLanguage == primaryLanguage);
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Author?> FindAuthorByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return _dbContext.Authors
            .FirstOrDefaultAsync(author => author.NormalizedName == normalizedName, cancellationToken);
    }

    public Task<Publisher?> FindPublisherByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return _dbContext.Publishers
            .FirstOrDefaultAsync(publisher => publisher.NormalizedName == normalizedName, cancellationToken);
    }

    public void AddAuthor(Author author)
    {
        _dbContext.Authors.Add(author);
    }

    public void AddPublisher(Publisher publisher)
    {
        _dbContext.Publishers.Add(publisher);
    }

    public void AddBookWork(BookWork bookWork)
    {
        _dbContext.BookWorks.Add(bookWork);
    }

    public void AddBookEdition(BookEdition bookEdition)
    {
        _dbContext.BookEditions.Add(bookEdition);
    }

    public void AddExternalBookReference(ExternalBookReference externalBookReference)
    {
        _dbContext.ExternalBookReferences.Add(externalBookReference);
    }

    public void AddBookEditionAuthor(BookEditionAuthor bookEditionAuthor)
    {
        _dbContext.BookEditionAuthors.Add(bookEditionAuthor);
    }

    private IQueryable<BookEdition> BookEditionGraph()
    {
        return _dbContext.BookEditions
            .Include(edition => edition.BookWork)
                .ThenInclude(work => work!.ExternalBookReferences)
            .Include(edition => edition.Publisher)
            .Include(edition => edition.ExternalBookReferences)
            .Include(edition => edition.BookEditionAuthors)
                .ThenInclude(link => link.Author)
            .AsSplitQuery();
    }

    private IQueryable<BookWork> BookWorkGraph()
    {
        return _dbContext.BookWorks
            .Include(work => work.ExternalBookReferences)
            .AsSplitQuery();
    }
}
