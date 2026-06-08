using System.Text.Json.Serialization;

namespace MeuAcervo.Infrastructure.Providers.OpenLibrary.Contracts;

public sealed class OpenLibrarySearchDocument
{
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("author_name")]
    public IReadOnlyCollection<string> AuthorNames { get; init; } = [];

    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishYear { get; init; }

    [JsonPropertyName("cover_i")]
    public int? CoverId { get; init; }

    [JsonPropertyName("editions")]
    public OpenLibraryEditionContainer? Editions { get; init; }
}
