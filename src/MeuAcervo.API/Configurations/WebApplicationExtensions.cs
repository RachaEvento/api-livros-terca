using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MeuAcervo.Infrastructure.Data;
using MeuAcervo.Shared.Configuration;
using System.Linq;

namespace MeuAcervo.API.Configurations;

public static class WebApplicationExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var databaseOptions = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        if (!databaseOptions.ApplyMigrationsOnStartup)
        {
            return;
        }

        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseMigration");

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToArray();

        if (pendingMigrations.Length == 0)
        {
            logger.LogInformation("No pending database migrations were found on startup.");
            return;
        }

        logger.LogInformation(
            "Applying {MigrationCount} pending database migrations on startup: {Migrations}.",
            pendingMigrations.Length,
            string.Join(", ", pendingMigrations));

        await dbContext.Database.MigrateAsync();
    }
}
