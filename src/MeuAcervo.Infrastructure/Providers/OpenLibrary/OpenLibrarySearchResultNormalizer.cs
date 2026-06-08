using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Application.Models.Books;
using MeuAcervo.Infrastructure.Providers.OpenLibrary.Contracts;
using MeuAcervo.Shared.Text;

namespace MeuAcervo.Infrastructure.Providers.OpenLibrary;

public sealed class OpenLibrarySearchResultNormalizer
    : IBookResultNormalizer<OpenLibrarySearchResponse, IReadOnlyCollection<ExternalBookSearchResult>>
{
    public const string SourceName = "open-library";

    public IReadOnlyCollection<ExternalBookSearchResult> Normalize(OpenLibrarySearchResponse source)
    {
        return source.Docs
            .Select(NormalizeDocument)
            .Where(result => result is not null)
            .Cast<ExternalBookSearchResult>()
            .ToArray();
    }

    private static ExternalBookSearchResult? NormalizeDocument(OpenLibrarySearchDocument document)
    {
        var edition = document.Editions?.Docs.FirstOrDefault();
        var editionId = ExtractOlid(edition?.Key);
        var workId = ExtractOlid(document.Key);

        if (string.IsNullOrWhiteSpace(editionId) && string.IsNullOrWhiteSpace(workId))
        {
            return null;
        }

        var isbnValues = edition?.Isbn
            .Select(TextNormalizationHelper.NormalizeIsbn)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
            ?? [];

        var isbn13 = isbnValues.FirstOrDefault(value => value.Length == 13);
        var isbn10 = isbnValues.FirstOrDefault(value => value.Length == 10);
        var language = edition?.Language
            .Select(TextNormalizationHelper.NormalizeLanguageCode)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        var publisher = edition?.Publisher.FirstOrDefault();
        var publishedYear = edition?.PublishYear.OrderBy(year => year).FirstOrDefault();
        var displayTitle = edition?.Title ?? document.Title ?? workId ?? editionId!;

        return new ExternalBookSearchResult(
            SourceName,
            editionId ?? workId!,
            workId,
            displayTitle,
            document.Title,
            null,
            document.AuthorNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            isbn10,
            isbn13,
            publisher,
            publishedYear > 0 ? publishedYear : null,
            document.FirstPublishYear,
            language,
            null,
            BuildCoverImageUrl(editionId, document.CoverId),
            BuildExternalUrl(editionId, workId),
            null,
            0m);
    }

    private static string? ExtractOlid(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        var trimmed = key.Trim();
        var lastSlashIndex = trimmed.LastIndexOf('/');

        return lastSlashIndex >= 0
            ? trimmed[(lastSlashIndex + 1)..]
            : trimmed;
    }

    private static string? BuildCoverImageUrl(string? editionId, int? coverId)
    {
        if (!string.IsNullOrWhiteSpace(editionId))
        {
            return $"https://covers.openlibrary.org/b/olid/{editionId}-L.jpg";
        }

        if (coverId.HasValue)
        {
            return $"https://covers.openlibrary.org/b/id/{coverId.Value}-L.jpg";
        }

        return null;
    }

    private static string? BuildExternalUrl(string? editionId, string? workId)
    {
        if (!string.IsNullOrWhiteSpace(editionId))
        {
            return $"https://openlibrary.org/books/{editionId}";
        }

        if (!string.IsNullOrWhiteSpace(workId))
        {
            return $"https://openlibrary.org/works/{workId}";
        }

        return null;
    }
}
