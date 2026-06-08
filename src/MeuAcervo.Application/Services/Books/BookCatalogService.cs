using AutoMapper;
using FluentValidation;
using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Books;
using MeuAcervo.Application.DTOs.Catalog;
using MeuAcervo.Application.Models.Books;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;
using MeuAcervo.Shared.Pagination;
using MeuAcervo.Shared.Text;

namespace MeuAcervo.Application.Services.Books;

public sealed class BookCatalogService : IBookCatalogService
{
    private readonly IValidator<BookSearchRequest> _bookSearchRequestValidator;
    private readonly IValidator<BookImportRequest> _bookImportRequestValidator;
    private readonly IBookSearchOrchestrator _bookSearchOrchestrator;
    private readonly IBookCatalogRepository _bookCatalogRepository;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public BookCatalogService(
        IValidator<BookSearchRequest> bookSearchRequestValidator,
        IValidator<BookImportRequest> bookImportRequestValidator,
        IBookSearchOrchestrator bookSearchOrchestrator,
        IBookCatalogRepository bookCatalogRepository,
        IApplicationDbContext applicationDbContext,
        IMapper mapper)
    {
        _bookSearchRequestValidator = bookSearchRequestValidator;
        _bookImportRequestValidator = bookImportRequestValidator;
        _bookSearchOrchestrator = bookSearchOrchestrator;
        _bookCatalogRepository = bookCatalogRepository;
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<PagedResult<BookSearchResultResponse>> SearchAsync(BookSearchRequest request, CancellationToken cancellationToken = default)
    {
        await _bookSearchRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var query = new BookSearchQuery(
            request.Search?.Trim(),
            NormalizeIsbnOrNull(request.Isbn),
            request.Title?.Trim(),
            request.Author?.Trim(),
            NormalizeLanguageOrNull(request.Language),
            request.PageNumber,
            request.PageSize);

        var searchResult = await _bookSearchOrchestrator.SearchAsync(query, cancellationToken);
        var responseItems = searchResult.Items
            .Select(MapSearchResult)
            .ToArray();

        return new PagedResult<BookSearchResultResponse>(
            responseItems,
            searchResult.PageNumber,
            searchResult.PageSize,
            searchResult.TotalCount);
    }

    public async Task<BookImportResponse> ImportAsync(BookImportRequest request, CancellationToken cancellationToken = default)
    {
        await _bookImportRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var normalizedSource = request.Source.Trim().ToLowerInvariant();
        var normalizedEditionId = NormalizeExternalIdOrNull(request.ExternalId);
        var normalizedWorkExternalId = NormalizeExternalIdOrNull(request.WorkExternalId);
        var normalizedTitle = TextNormalizationHelper.NormalizeText(request.Title);
        var normalizedWorkTitle = TextNormalizationHelper.NormalizeText(request.WorkTitle ?? request.Title);
        var normalizedPublisher = TextNormalizationHelper.NormalizeText(request.Publisher);
        var normalizedLanguage = NormalizeLanguageOrNull(request.Language);
        var normalizedIsbn10 = NormalizeIsbnOrNull(request.Isbn10);
        var normalizedIsbn13 = NormalizeIsbnOrNull(request.Isbn13);
        var authorNames = request.Authors
            .Where(author => !string.IsNullOrWhiteSpace(author))
            .Select(author => author.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var normalizedAuthorNames = authorNames
            .Select(TextNormalizationHelper.NormalizeText)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        var edition = await ResolveEditionAsync(
            normalizedSource,
            normalizedEditionId,
            normalizedIsbn13,
            normalizedIsbn10,
            normalizedTitle,
            normalizedLanguage,
            normalizedPublisher,
            normalizedAuthorNames,
            cancellationToken);

        var work = edition?.BookWork;
        var workFromReference = normalizedWorkExternalId is null
            ? null
            : await _bookCatalogRepository.GetWorkByExternalReferenceAsync(normalizedSource, normalizedWorkExternalId, cancellationToken);

        if (edition is not null && workFromReference is not null && edition.BookWorkId != workFromReference.Id)
        {
            throw new ConflictException("The external work reference is already linked to a different work in the catalog.");
        }

        work ??= workFromReference
                 ?? await _bookCatalogRepository.FindWorkByNormalizedTitleAsync(normalizedWorkTitle, normalizedLanguage, cancellationToken);

        var createdNewWork = false;
        if (work is null)
        {
            work = new BookWork
            {
                CanonicalTitle = request.WorkTitle?.Trim() ?? request.Title.Trim(),
                NormalizedCanonicalTitle = normalizedWorkTitle,
                Description = request.Description?.Trim(),
                FirstPublicationYear = request.FirstPublishedYear ?? request.PublishedYear,
                PrimaryLanguage = normalizedLanguage
            };

            _bookCatalogRepository.AddBookWork(work);
            createdNewWork = true;
        }
        else
        {
            EnrichWork(work, request, normalizedWorkTitle, normalizedLanguage);
        }

        var publisher = await ResolvePublisherAsync(request.Publisher, normalizedPublisher, cancellationToken);
        var createdNewEdition = false;

        if (edition is null)
        {
            edition = new BookEdition
            {
                BookWorkId = work.Id,
                BookWork = work,
                Isbn10 = normalizedIsbn10,
                Isbn13 = normalizedIsbn13,
                Title = request.Title.Trim(),
                NormalizedTitle = normalizedTitle,
                Subtitle = request.Subtitle?.Trim(),
                PublisherId = publisher?.Id,
                Publisher = publisher,
                PublishedAt = TextNormalizationHelper.CreatePublishedAtUtc(request.PublishedYear),
                PageCount = request.PageCount,
                Language = normalizedLanguage ?? string.Empty,
                CoverImageUrl = request.CoverImageUrl?.Trim()
            };

            _bookCatalogRepository.AddBookEdition(edition);
            work.BookEditions.Add(edition);
            createdNewEdition = true;
        }
        else
        {
            EnrichEdition(edition, work, publisher, request, normalizedTitle, normalizedLanguage, normalizedIsbn10, normalizedIsbn13);
        }

        var authors = await ResolveAuthorsAsync(authorNames, normalizedAuthorNames, cancellationToken);
        EnsureEditionAuthorLinks(edition, authors);
        EnsureExternalReferences(work, edition, normalizedSource, normalizedWorkExternalId, normalizedEditionId, request);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        var externalReferences = work.ExternalBookReferences
            .Concat(edition.ExternalBookReferences)
            .OrderBy(reference => reference.ReferenceType)
            .ThenBy(reference => reference.Provider, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new BookImportResponse(
            createdNewWork,
            createdNewEdition,
            _mapper.Map<BookWorkSummaryResponse>(work),
            _mapper.Map<BookEditionSummaryResponse>(edition),
            publisher is null ? null : _mapper.Map<PublisherSummaryResponse>(publisher),
            _mapper.Map<IReadOnlyCollection<AuthorSummaryResponse>>(authors),
            _mapper.Map<IReadOnlyCollection<ExternalBookReferenceSummaryResponse>>(externalReferences));
    }

    private async Task<BookEdition?> ResolveEditionAsync(
        string provider,
        string? externalId,
        string? isbn13,
        string? isbn10,
        string normalizedTitle,
        string? normalizedLanguage,
        string normalizedPublisher,
        IReadOnlyCollection<string> normalizedAuthorNames,
        CancellationToken cancellationToken)
    {
        if (externalId is not null)
        {
            var byExternalReference = await _bookCatalogRepository.GetEditionByExternalReferenceAsync(provider, externalId, cancellationToken);
            if (byExternalReference is not null)
            {
                return byExternalReference;
            }
        }

        if (isbn13 is not null)
        {
            var byIsbn13 = await _bookCatalogRepository.GetEditionByIsbn13Async(isbn13, cancellationToken);
            if (byIsbn13 is not null)
            {
                return byIsbn13;
            }
        }

        if (isbn10 is not null)
        {
            var byIsbn10 = await _bookCatalogRepository.GetEditionByIsbn10Async(isbn10, cancellationToken);
            if (byIsbn10 is not null)
            {
                return byIsbn10;
            }
        }

        var candidates = await _bookCatalogRepository.FindEditionsByNormalizedTitleAsync(normalizedTitle, normalizedLanguage, cancellationToken);
        return SelectBestEditionCandidate(candidates, normalizedPublisher, normalizedAuthorNames);
    }

    private async Task<Publisher?> ResolvePublisherAsync(string? publisherName, string normalizedPublisher, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(publisherName) || string.IsNullOrWhiteSpace(normalizedPublisher))
        {
            return null;
        }

        var publisher = await _bookCatalogRepository.FindPublisherByNormalizedNameAsync(normalizedPublisher, cancellationToken);
        if (publisher is not null)
        {
            return publisher;
        }

        publisher = new Publisher
        {
            Name = publisherName.Trim(),
            NormalizedName = normalizedPublisher
        };

        _bookCatalogRepository.AddPublisher(publisher);
        return publisher;
    }

    private async Task<IReadOnlyCollection<Author>> ResolveAuthorsAsync(
        IReadOnlyList<string> authorNames,
        IReadOnlyList<string> normalizedAuthorNames,
        CancellationToken cancellationToken)
    {
        var authors = new List<Author>(authorNames.Count);

        for (var index = 0; index < authorNames.Count; index++)
        {
            var authorName = authorNames[index];
            var normalizedAuthorName = normalizedAuthorNames[index];
            var author = await _bookCatalogRepository.FindAuthorByNormalizedNameAsync(normalizedAuthorName, cancellationToken);

            if (author is null)
            {
                author = new Author
                {
                    Name = authorName,
                    NormalizedName = normalizedAuthorName
                };

                _bookCatalogRepository.AddAuthor(author);
            }

            authors.Add(author);
        }

        return authors;
    }

    private void EnsureEditionAuthorLinks(BookEdition edition, IReadOnlyCollection<Author> authors)
    {
        var nextContributionOrder = edition.BookEditionAuthors.Count == 0
            ? 1
            : edition.BookEditionAuthors.Max(link => link.ContributionOrder) + 1;

        foreach (var author in authors)
        {
            var alreadyLinked = edition.BookEditionAuthors.Any(link => link.AuthorId == author.Id);
            if (alreadyLinked)
            {
                continue;
            }

            var link = new BookEditionAuthor
            {
                BookEditionId = edition.Id,
                BookEdition = edition,
                AuthorId = author.Id,
                Author = author,
                ContributionOrder = nextContributionOrder++
            };

            edition.BookEditionAuthors.Add(link);
            author.BookEditionAuthors.Add(link);
            _bookCatalogRepository.AddBookEditionAuthor(link);
        }
    }

    private void EnsureExternalReferences(
        BookWork work,
        BookEdition edition,
        string provider,
        string? workExternalId,
        string? editionExternalId,
        BookImportRequest request)
    {
        if (!string.IsNullOrWhiteSpace(workExternalId) && work.ExternalBookReferences.All(reference =>
                !string.Equals(reference.Provider, provider, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(reference.ExternalId, workExternalId, StringComparison.OrdinalIgnoreCase)
                || reference.ReferenceType != ExternalBookReferenceType.Work))
        {
            var workReference = new ExternalBookReference
            {
                Provider = provider,
                ExternalId = workExternalId,
                ReferenceType = ExternalBookReferenceType.Work,
                BookWorkId = work.Id,
                BookWork = work,
                ExternalUrl = BuildReferenceUrl(request.ExternalUrl, provider, workExternalId, true)
            };

            work.ExternalBookReferences.Add(workReference);
            _bookCatalogRepository.AddExternalBookReference(workReference);
        }

        if (!string.IsNullOrWhiteSpace(editionExternalId) && edition.ExternalBookReferences.All(reference =>
                !string.Equals(reference.Provider, provider, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(reference.ExternalId, editionExternalId, StringComparison.OrdinalIgnoreCase)
                || reference.ReferenceType != ExternalBookReferenceType.Edition))
        {
            var editionReference = new ExternalBookReference
            {
                Provider = provider,
                ExternalId = editionExternalId,
                ReferenceType = ExternalBookReferenceType.Edition,
                BookEditionId = edition.Id,
                BookEdition = edition,
                ExternalUrl = BuildReferenceUrl(request.ExternalUrl, provider, editionExternalId, false)
            };

            edition.ExternalBookReferences.Add(editionReference);
            _bookCatalogRepository.AddExternalBookReference(editionReference);
        }
    }

    private static BookEdition? SelectBestEditionCandidate(
        IEnumerable<BookEdition> candidates,
        string normalizedPublisher,
        IReadOnlyCollection<string> normalizedAuthorNames)
    {
        return candidates
            .Select(candidate => new
            {
                Candidate = candidate,
                Score = ScoreEditionCandidate(candidate, normalizedPublisher, normalizedAuthorNames)
            })
            .Where(item => item.Score > 0)
            .OrderByDescending(item => item.Score)
            .ThenByDescending(item => item.Candidate.UpdatedAtUtc)
            .Select(item => item.Candidate)
            .FirstOrDefault();
    }

    private static int ScoreEditionCandidate(
        BookEdition candidate,
        string normalizedPublisher,
        IReadOnlyCollection<string> normalizedAuthorNames)
    {
        var score = 1;

        if (!string.IsNullOrWhiteSpace(normalizedPublisher)
            && string.Equals(candidate.Publisher?.NormalizedName, normalizedPublisher, StringComparison.OrdinalIgnoreCase))
        {
            score += 5;
        }

        if (normalizedAuthorNames.Count == 0)
        {
            return score;
        }

        var candidateAuthorNames = candidate.BookEditionAuthors
            .Select(link => link.Author?.NormalizedName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var authorMatches = normalizedAuthorNames.Count(candidateAuthorNames.Contains);
        score += authorMatches * 3;

        return authorMatches > 0 ? score : 0;
    }

    private static void EnrichWork(BookWork work, BookImportRequest request, string normalizedWorkTitle, string? normalizedLanguage)
    {
        if (string.IsNullOrWhiteSpace(work.CanonicalTitle))
        {
            work.CanonicalTitle = request.WorkTitle?.Trim() ?? request.Title.Trim();
        }

        if (string.IsNullOrWhiteSpace(work.NormalizedCanonicalTitle))
        {
            work.NormalizedCanonicalTitle = normalizedWorkTitle;
        }

        work.Description ??= request.Description?.Trim();
        work.FirstPublicationYear ??= request.FirstPublishedYear ?? request.PublishedYear;
        work.PrimaryLanguage ??= normalizedLanguage;
    }

    private static void EnrichEdition(
        BookEdition edition,
        BookWork work,
        Publisher? publisher,
        BookImportRequest request,
        string normalizedTitle,
        string? normalizedLanguage,
        string? normalizedIsbn10,
        string? normalizedIsbn13)
    {
        edition.BookWorkId = work.Id;
        edition.BookWork = work;
        edition.Isbn10 ??= normalizedIsbn10;
        edition.Isbn13 ??= normalizedIsbn13;
        edition.Title = string.IsNullOrWhiteSpace(edition.Title) ? request.Title.Trim() : edition.Title;
        edition.NormalizedTitle = string.IsNullOrWhiteSpace(edition.NormalizedTitle) ? normalizedTitle : edition.NormalizedTitle;
        edition.Subtitle ??= request.Subtitle?.Trim();
        edition.PublisherId ??= publisher?.Id;
        edition.Publisher ??= publisher;
        edition.PublishedAt ??= TextNormalizationHelper.CreatePublishedAtUtc(request.PublishedYear);
        edition.PageCount ??= request.PageCount;
        edition.Language = string.IsNullOrWhiteSpace(edition.Language)
            ? normalizedLanguage ?? string.Empty
            : edition.Language;
        edition.CoverImageUrl ??= request.CoverImageUrl?.Trim();
    }

    private static BookSearchResultResponse MapSearchResult(ExternalBookSearchResult result)
    {
        return new BookSearchResultResponse(
            result.Source,
            result.ExternalId,
            result.WorkExternalId,
            result.Title,
            result.WorkTitle,
            result.Subtitle,
            result.Authors,
            result.Isbn10,
            result.Isbn13,
            result.Publisher,
            result.PublishedYear,
            result.FirstPublishedYear,
            result.Language,
            result.PageCount,
            result.CoverImageUrl,
            result.ExternalUrl,
            result.ConfidenceScore);
    }

    private static string? NormalizeIsbnOrNull(string? value)
    {
        var normalized = TextNormalizationHelper.NormalizeIsbn(value);
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string? NormalizeLanguageOrNull(string? value)
    {
        var normalized = TextNormalizationHelper.NormalizeLanguageCode(value);
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string? NormalizeExternalIdOrNull(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string? BuildReferenceUrl(string? externalUrl, string provider, string externalId, bool isWorkReference)
    {
        if (!string.IsNullOrWhiteSpace(externalUrl))
        {
            return externalUrl.Trim();
        }

        if (!string.Equals(provider, "open-library", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var pathSegment = isWorkReference ? "works" : "books";
        return $"https://openlibrary.org/{pathSegment}/{externalId}";
    }
}
