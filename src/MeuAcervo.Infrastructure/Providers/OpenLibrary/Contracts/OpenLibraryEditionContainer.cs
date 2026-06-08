using System.Text.Json.Serialization;

namespace MeuAcervo.Infrastructure.Providers.OpenLibrary.Contracts;

public sealed class OpenLibraryEditionContainer
{
    [JsonPropertyName("docs")]
    public IReadOnlyCollection<OpenLibraryEditionDocument> Docs { get; init; } = [];
}
