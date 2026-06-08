using Microsoft.Extensions.Configuration;
using Npgsql;

namespace MeuAcervo.Infrastructure.Data;

public static class DatabaseConnectionStringResolver
{
    public static string Resolve(IConfiguration? configuration = null)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return ConvertDatabaseUrlToNpgsqlConnectionString(databaseUrl);
        }

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                               ?? configuration?.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("DATABASE_URL or ConnectionStrings:DefaultConnection must be configured.");
        }

        return connectionString;
    }

    private static string ConvertDatabaseUrlToNpgsqlConnectionString(string databaseUrl)
    {
        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException("DATABASE_URL is not a valid absolute URI.");
        }

        if (!string.Equals(uri.Scheme, "postgresql", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(uri.Scheme, "postgres", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("DATABASE_URL must use the postgres or postgresql scheme.");
        }

        var userInfo = uri.UserInfo.Split(':', 2);
        if (userInfo.Length != 2)
        {
            throw new InvalidOperationException("DATABASE_URL must contain username and password.");
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = uri.AbsolutePath.Trim('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = Uri.UnescapeDataString(userInfo[1]),
            SslMode = SslMode.Prefer
        };

        var queryParameters = ParseQueryString(uri.Query);

        if (queryParameters.TryGetValue("schema", out var schema) && !string.IsNullOrWhiteSpace(schema))
        {
            builder.SearchPath = schema;
        }

        if (queryParameters.TryGetValue("sslmode", out var sslMode) &&
            Enum.TryParse<SslMode>(sslMode, ignoreCase: true, out var parsedSslMode))
        {
            builder.SslMode = parsedSslMode;
        }

        return builder.ConnectionString;
    }

    private static Dictionary<string, string> ParseQueryString(string queryString)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(queryString))
        {
            return result;
        }

        var trimmedQuery = queryString.TrimStart('?');
        foreach (var pair in trimmedQuery.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(parts[0]).Replace('+', ' ');
            var value = parts.Length > 1
                ? Uri.UnescapeDataString(parts[1]).Replace('+', ' ')
                : string.Empty;

            result[key] = value;
        }

        return result;
    }
}
