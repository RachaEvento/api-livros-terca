using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Application.DTOs.Books;
using MeuAcervo.Application.Models.Books;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Shared.Text;

namespace MeuAcervo.Application.Services.Books;

public sealed class BookEditionMatcher : IBookEditionMatcher
{
    private readonly IBookCatalogRepository _bookCatalogRepository;

    public BookEditionMatcher(IBookCatalogRepository bookCatalogRepository)
    {
        _bookCatalogRepository = bookCatalogRepository;
    }

    public async Task<BookEdition?> FindMatchingEditionAsync(BookImportRequest request, CancellationToken cancellationToken = default)
    {
        var matches = await FindMatchingEditionsAsync([request], cancellationToken);
        return matches[0];
    }

    public async Task<IReadOnlyList<BookEdition?>> FindMatchingEditionsAsync(
        IReadOnlyList<BookImportRequest> requests,
        CancellationToken cancellationToken = default)
    {
        if (requests.Count == 0)
        {
            return Array.Empty<BookEdition?>();
        }

        var criteria = requests
            .Select(CreateCriteria)
            .ToArray();

        var externalReferenceLookups = criteria
            .Where(item => item.ExternalId is not null)
            .Select(item => new EditionExternalReferenceLookup(item.Provider, item.ExternalId!))
            .Distinct()
            .ToArray();

        var isbn13Values = criteria
            .Where(item => item.Isbn13 is not null)
            .Select(item => item.Isbn13!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var isbn10Values = criteria
            .Where(item => item.Isbn10 is not null)
            .Select(item => item.Isbn10!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var normalizedTitles = criteria
            .Select(item => item.NormalizedTitle)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var editionsByExternalReference = (await _bookCatalogRepository.GetEditionsByExternalReferencesAsync(externalReferenceLookups, cancellationToken))
            .SelectMany(edition => edition.ExternalBookReferences
                .Where(reference => reference.ReferenceType == Domain.Enums.ExternalBookReferenceType.Edition)
                .Select(reference => new
                {
                    Key = CreateExternalReferenceKey(reference.Provider, reference.ExternalId),
                    Edition = edition
                }))
            .GroupBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Edition, StringComparer.OrdinalIgnoreCase);

        var editionsByIsbn13 = (await _bookCatalogRepository.GetEditionsByIsbn13Async(isbn13Values, cancellationToken))
            .Where(edition => !string.IsNullOrWhiteSpace(edition.Isbn13))
            .GroupBy(edition => edition.Isbn13!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        var editionsByIsbn10 = (await _bookCatalogRepository.GetEditionsByIsbn10Async(isbn10Values, cancellationToken))
            .Where(edition => !string.IsNullOrWhiteSpace(edition.Isbn10))
            .GroupBy(edition => edition.Isbn10!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        var editionsByNormalizedTitle = (await _bookCatalogRepository.FindEditionsByNormalizedTitlesAsync(normalizedTitles, cancellationToken))
            .GroupBy(edition => edition.NormalizedTitle, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => (IReadOnlyCollection<BookEdition>)group.ToArray(), StringComparer.OrdinalIgnoreCase);

        var matches = new BookEdition?[criteria.Length];

        for (var index = 0; index < criteria.Length; index++)
        {
            var candidate = criteria[index];

            if (candidate.ExternalId is not null
                && editionsByExternalReference.TryGetValue(CreateExternalReferenceKey(candidate.Provider, candidate.ExternalId), out var byExternalReference))
            {
                matches[index] = byExternalReference;
                continue;
            }

            if (candidate.Isbn13 is not null && editionsByIsbn13.TryGetValue(candidate.Isbn13, out var byIsbn13))
            {
                matches[index] = byIsbn13;
                continue;
            }

            if (candidate.Isbn10 is not null && editionsByIsbn10.TryGetValue(candidate.Isbn10, out var byIsbn10))
            {
                matches[index] = byIsbn10;
                continue;
            }

            if (!editionsByNormalizedTitle.TryGetValue(candidate.NormalizedTitle, out var titleCandidates))
            {
                continue;
            }

            var filteredCandidates = string.IsNullOrWhiteSpace(candidate.Language)
                ? titleCandidates
                : titleCandidates
                    .Where(edition => string.Equals(edition.Language, candidate.Language, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

            matches[index] = SelectBestEditionCandidate(filteredCandidates, candidate.NormalizedPublisher, candidate.NormalizedAuthorNames);
        }

        return matches;
    }

    private static BookEditionMatchCriteria CreateCriteria(BookImportRequest request)
    {
        var normalizedAuthorNames = request.Authors
            .Where(author => !string.IsNullOrWhiteSpace(author))
            .Select(author => TextNormalizationHelper.NormalizeText(author.Trim()))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new BookEditionMatchCriteria(
            request.Source.Trim().ToLowerInvariant(),
            NormalizeExternalIdOrNull(request.ExternalId),
            NormalizeIsbnOrNull(request.Isbn13),
            NormalizeIsbnOrNull(request.Isbn10),
            TextNormalizationHelper.NormalizeText(request.Title),
            NormalizeLanguageOrNull(request.Language),
            TextNormalizationHelper.NormalizeText(request.Publisher),
            normalizedAuthorNames);
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

    private static string CreateExternalReferenceKey(string provider, string externalId)
    {
        return $"{provider.Trim().ToLowerInvariant()}::{externalId.Trim().ToLowerInvariant()}";
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

    private sealed record BookEditionMatchCriteria(
        string Provider,
        string? ExternalId,
        string? Isbn13,
        string? Isbn10,
        string NormalizedTitle,
        string? Language,
        string NormalizedPublisher,
        IReadOnlyCollection<string> NormalizedAuthorNames);
}
