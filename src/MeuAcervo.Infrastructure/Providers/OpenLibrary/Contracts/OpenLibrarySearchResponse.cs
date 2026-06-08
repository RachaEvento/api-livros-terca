using System.Text.Json.Serialization;

namespace MeuAcervo.Infrastructure.Providers.OpenLibrary.Contracts;

public sealed class OpenLibrarySearchResponse
{
    [JsonPropertyName("numFound")]
    public int? NumFound { get; init; }

    [JsonPropertyName("num_found")]
    public int? LegacyNumFound { get; init; }

    [JsonPropertyName("docs")]
    public IReadOnlyCollection<OpenLibrarySearchDocument> Docs { get; init; } = [];

    public int EffectiveNumFound => NumFound ?? LegacyNumFound ?? Docs.Count;
}
