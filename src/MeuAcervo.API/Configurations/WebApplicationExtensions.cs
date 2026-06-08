using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MeuAcervo.Infrastructure.Data;
using MeuAcervo.Shared.Configuration;

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

        logger.LogInformation("Applying database migrations on startup.");

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
