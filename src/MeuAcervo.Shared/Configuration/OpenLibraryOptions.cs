using System.ComponentModel.DataAnnotations;

namespace MeuAcervo.Shared.Configuration;

public sealed class OpenLibraryOptions
{
    public const string SectionName = "BookProviders:OpenLibrary";

    public bool Enabled { get; init; } = true;

    [Required]
    [Url]
    public string BaseUrl { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string UserAgent { get; init; } = string.Empty;

    [EmailAddress]
    public string? ContactEmail { get; init; }

    [Range(1, 60)]
    public int TimeoutSeconds { get; init; } = 8;
}
