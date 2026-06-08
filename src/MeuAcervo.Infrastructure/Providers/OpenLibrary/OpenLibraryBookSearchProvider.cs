using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MeuAcervo.Application.Abstractions.Books;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.Models.Books;
using MeuAcervo.Infrastructure.Providers.OpenLibrary.Contracts;
using MeuAcervo.Shared.Configuration;
using MeuAcervo.Shared.Pagination;
using MeuAcervo.Shared.Text;

namespace MeuAcervo.Infrastructure.Providers.OpenLibrary;

public sealed class OpenLibraryBookSearchProvider : IBookSearchProvider
{
    private const string SearchFields =
        "key,title,author_name,first_publish_year,cover_i,editions,editions.key,editions.title,editions.language,editions.publisher,editions.publish_year,editions.isbn";

    private readonly HttpClient _httpClient;
    private readonly OpenLibraryOptions _options;
    private readonly IBookResultNormalizer<OpenLibrarySearchResponse, IReadOnlyCollection<ExternalBookSearchResult>> _normalizer;
    private readonly ILogger<OpenLibraryBookSearchProvider> _logger;

    public OpenLibraryBookSearchProvider(
        HttpClient httpClient,
        IOptions<OpenLibraryOptions> options,
        IBookResultNormalizer<OpenLibrarySearchResponse, IReadOnlyCollection<ExternalBookSearchResult>> normalizer,
        ILogger<OpenLibraryBookSearchProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _normalizer = normalizer;
        _logger = logger;
    }

    public string Source => OpenLibrarySearchResultNormalizer.SourceName;

    public async Task<BookSearchProviderPage> SearchAsync(BookSearchQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = BuildRequestUri(query);
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Open Library search failed with status code {StatusCode}. Query: {Query}",
                    (int)responseMessage.StatusCode,
                    requestUri);

                throw new ExternalProviderException("Open Library could not process the search request.");
            }

            var payload = await responseMessage.Content.ReadFromJsonAsync<OpenLibrarySearchResponse>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                throw new ExternalProviderException("Open Library returned an empty search response.");
            }

            var results = _normalizer.Normalize(payload)
                .Select((result, index) => ApplyConfidence(result, query, index))
                .ToArray();

            return new BookSearchProviderPage(
                Source,
                new PagedResult<ExternalBookSearchResult>(
                    results,
                    query.PageNumber,
                    query.PageSize,
                    payload.EffectiveNumFound));
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(exception, "Open Library search timed out.");
            throw new ExternalProviderException("Open Library timed out while searching for books.", exception);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Open Library search failed due to an HTTP transport error.");
            throw new ExternalProviderException("Open Library is unavailable at the moment.", exception);
        }
    }

    private string BuildRequestUri(BookSearchQuery query)
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("fields", SearchFields),
            new("page", query.PageNumber.ToString(CultureInfo.InvariantCulture)),
            new("limit", query.PageSize.ToString(CultureInfo.InvariantCulture))
        };

        if (!string.IsNullOrWhiteSpace(_options.ContactEmail))
        {
            parameters.Add(new KeyValuePair<string, string>("email", _options.ContactEmail.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(query.Language))
        {
            parameters.Add(new KeyValuePair<string, string>("lang", query.Language));
        }

        if (!string.IsNullOrWhiteSpace(query.Isbn))
        {
            parameters.Add(new KeyValuePair<string, string>("isbn", query.Isbn));
        }
        else if (!string.IsNullOrWhiteSpace(query.Title) && !string.IsNullOrWhiteSpace(query.Author))
        {
            parameters.Add(new KeyValuePair<string, string>("title", query.Title));
            parameters.Add(new KeyValuePair<string, string>("author", query.Author));
        }
        else if (!string.IsNullOrWhiteSpace(query.Title))
        {
            parameters.Add(new KeyValuePair<string, string>("title", query.Title));
        }
        else if (!string.IsNullOrWhiteSpace(query.Author))
        {
            parameters.Add(new KeyValuePair<string, string>("author", query.Author));
        }
        else if (!string.IsNullOrWhiteSpace(query.Search))
        {
            parameters.Add(new KeyValuePair<string, string>("q", query.Search));
        }

        var builder = new StringBuilder("search.json?");
        builder.Append(string.Join("&", parameters.Select(parameter =>
            $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value)}")));

        return builder.ToString();
    }

    private static ExternalBookSearchResult ApplyConfidence(ExternalBookSearchResult result, BookSearchQuery query, int index)
    {
        var score = 0.55m;
        score += Math.Max(0m, 0.18m - (index * 0.02m));

        if (!string.IsNullOrWhiteSpace(result.WorkExternalId))
        {
            score += 0.04m;
        }

        if (!string.IsNullOrWhiteSpace(result.Isbn13) || !string.IsNullOrWhiteSpace(result.Isbn10))
        {
            score += 0.08m;
        }

        if (result.Authors.Count > 0)
        {
            score += 0.04m;
        }

        if (!string.IsNullOrWhiteSpace(result.Publisher))
        {
            score += 0.03m;
        }

        if (!string.IsNullOrWhiteSpace(result.CoverImageUrl))
        {
            score += 0.02m;
        }

        if (!string.IsNullOrWhiteSpace(query.Isbn))
        {
            var requestedIsbn = TextNormalizationHelper.NormalizeIsbn(query.Isbn);
            if (string.Equals(result.Isbn13, requestedIsbn, StringComparison.OrdinalIgnoreCase)
                || string.Equals(result.Isbn10, requestedIsbn, StringComparison.OrdinalIgnoreCase))
            {
                score += 0.08m;
            }
        }

        return result with
        {
            ConfidenceScore = decimal.Round(Math.Min(score, 0.99m), 2, MidpointRounding.AwayFromZero)
        };
    }
}
