using System.ComponentModel.DataAnnotations;

namespace MeuAcervo.Shared.Configuration;

public sealed class AppInfoOptions
{
    public const string SectionName = "AppInfo";

    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string Version { get; init; } = string.Empty;

    [Required]
    public string Description { get; init; } = string.Empty;
}
