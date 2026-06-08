using Microsoft.Extensions.Logging;
using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.Models.Books;
using MeuAcervo.Shared.Pagination;
using MeuAcervo.Shared.Text;

namespace MeuAcervo.Application.Services.Books;

public sealed class BookSearchOrchestrator : IBookSearchOrchestrator
{
    private readonly IReadOnlyCollection<IBookSearchProvider> _providers;
    private readonly ILogger<BookSearchOrchestrator> _logger;

    public BookSearchOrchestrator(IEnumerable<IBookSearchProvider> providers, ILogger<BookSearchOrchestrator> logger)
    {
        _providers = providers.ToArray();
        _logger = logger;
    }

    public async Task<PagedResult<ExternalBookSearchResult>> SearchAsync(BookSearchQuery query, CancellationToken cancellationToken = default)
    {
        if (_providers.Count == 0)
        {
            throw new ExternalProviderException("No book search providers are enabled.");
        }

        var searchTasks = _providers
            .Select(provider => SearchSafelyAsync(provider, query, cancellationToken))
            .ToArray();

        var providerPages = await Task.WhenAll(searchTasks);
        var successfulPages = providerPages
            .Where(page => page is not null)
            .Cast<BookSearchProviderPage>()
            .ToArray();

        if (successfulPages.Length == 0)
        {
            throw new ExternalProviderException("No external book provider could complete the search at this time.");
        }

        var mergedResults = successfulPages
            .SelectMany(page => page.Results.Items)
            .ToArray();

        var deduplicatedResults = DeduplicateResults(mergedResults)
            .OrderByDescending(result => result.ConfidenceScore)
            .ThenBy(result => result.Title, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var totalCount = successfulPages.Length == 1
            ? successfulPages[0].Results.TotalCount
            : deduplicatedResults.Length;

        return new PagedResult<ExternalBookSearchResult>(
            deduplicatedResults,
            query.PageNumber,
            query.PageSize,
            totalCount);
    }

    private async Task<BookSearchProviderPage?> SearchSafelyAsync(
        IBookSearchProvider provider,
        BookSearchQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            return await provider.SearchAsync(query, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                exception,
                "Book provider {Provider} failed while executing search.",
                provider.Source);

            return null;
        }
    }

    private static IReadOnlyCollection<ExternalBookSearchResult> DeduplicateResults(IReadOnlyCollection<ExternalBookSearchResult> results)
    {
        return results
            .GroupBy(CreateDeduplicationKey, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var orderedGroup = group
                    .OrderByDescending(item => item.ConfidenceScore)
                    .ThenByDescending(GetCompletenessScore)
                    .ToArray();

                var primary = orderedGroup[0];
                return orderedGroup.Skip(1).Aggregate(primary, MergeResults);
            })
            .ToArray();
    }

    private static string CreateDeduplicationKey(ExternalBookSearchResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.Isbn13))
        {
            return $"isbn13:{TextNormalizationHelper.NormalizeIsbn(result.Isbn13)}";
        }

        if (!string.IsNullOrWhiteSpace(result.Isbn10))
        {
            return $"isbn10:{TextNormalizationHelper.NormalizeIsbn(result.Isbn10)}";
        }

        if (!string.IsNullOrWhiteSpace(result.ExternalId))
        {
            return $"external:{TextNormalizationHelper.NormalizeText(result.Source)}:{TextNormalizationHelper.NormalizeText(result.ExternalId)}";
        }

        var authors = string.Join('|', result.Authors.Select(TextNormalizationHelper.NormalizeText).Where(value => !string.IsNullOrWhiteSpace(value)));
        var publisher = TextNormalizationHelper.NormalizeText(result.Publisher);
        var title = TextNormalizationHelper.NormalizeText(result.Title);
        var language = TextNormalizationHelper.NormalizeLanguageCode(result.Language);

        return $"fallback:{title}:{authors}:{publisher}:{language}";
    }

    private static int GetCompletenessScore(ExternalBookSearchResult result)
    {
        var score = 0;

        if (!string.IsNullOrWhiteSpace(result.WorkExternalId))
        {
            score++;
        }

        if (!string.IsNullOrWhiteSpace(result.Isbn10))
        {
            score++;
        }

        if (!string.IsNullOrWhiteSpace(result.Isbn13))
        {
            score++;
        }

        if (!string.IsNullOrWhiteSpace(result.Publisher))
        {
            score++;
        }

        if (result.Authors.Count > 0)
        {
            score++;
        }

        if (result.PublishedYear.HasValue)
        {
            score++;
        }

        if (result.PageCount.HasValue)
        {
            score++;
        }

        if (!string.IsNullOrWhiteSpace(result.CoverImageUrl))
        {
            score++;
        }

        if (!string.IsNullOrWhiteSpace(result.Description))
        {
            score++;
        }

        return score;
    }

    private static ExternalBookSearchResult MergeResults(ExternalBookSearchResult primary, ExternalBookSearchResult duplicate)
    {
        var mergedAuthors = primary.Authors
            .Concat(duplicate.Authors)
            .Where(author => !string.IsNullOrWhiteSpace(author))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return primary with
        {
            WorkExternalId = Prefer(primary.WorkExternalId, duplicate.WorkExternalId),
            WorkTitle = Prefer(primary.WorkTitle, duplicate.WorkTitle),
            Subtitle = Prefer(primary.Subtitle, duplicate.Subtitle),
            Authors = mergedAuthors,
            Isbn10 = Prefer(primary.Isbn10, duplicate.Isbn10),
            Isbn13 = Prefer(primary.Isbn13, duplicate.Isbn13),
            Publisher = Prefer(primary.Publisher, duplicate.Publisher),
            PublishedYear = primary.PublishedYear ?? duplicate.PublishedYear,
            FirstPublishedYear = primary.FirstPublishedYear ?? duplicate.FirstPublishedYear,
            Language = Prefer(primary.Language, duplicate.Language),
            PageCount = primary.PageCount ?? duplicate.PageCount,
            CoverImageUrl = Prefer(primary.CoverImageUrl, duplicate.CoverImageUrl),
            ExternalUrl = Prefer(primary.ExternalUrl, duplicate.ExternalUrl),
            Description = Prefer(primary.Description, duplicate.Description),
            ConfidenceScore = Math.Max(primary.ConfidenceScore, duplicate.ConfidenceScore)
        };
    }

    private static string? Prefer(string? primaryValue, string? secondaryValue)
    {
        return string.IsNullOrWhiteSpace(primaryValue)
            ? secondaryValue
            : primaryValue;
    }
}
