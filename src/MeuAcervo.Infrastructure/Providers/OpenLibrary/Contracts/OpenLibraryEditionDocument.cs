using System.Text.Json.Serialization;

namespace MeuAcervo.Infrastructure.Providers.OpenLibrary.Contracts;

public sealed class OpenLibraryEditionDocument
{
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("language")]
    public IReadOnlyCollection<string> Language { get; init; } = [];

    [JsonPropertyName("publisher")]
    public IReadOnlyCollection<string> Publisher { get; init; } = [];

    [JsonPropertyName("publish_year")]
    public IReadOnlyCollection<int> PublishYear { get; init; } = [];

    [JsonPropertyName("isbn")]
    public IReadOnlyCollection<string> Isbn { get; init; } = [];
}
