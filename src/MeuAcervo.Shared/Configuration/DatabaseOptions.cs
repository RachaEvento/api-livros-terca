namespace MeuAcervo.Shared.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public bool ApplyMigrationsOnStartup { get; init; } = true;
}
