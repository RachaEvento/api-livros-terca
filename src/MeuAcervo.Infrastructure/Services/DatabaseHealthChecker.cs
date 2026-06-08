using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Infrastructure.Data;

namespace MeuAcervo.Infrastructure.Services;

public sealed class DatabaseHealthChecker : IDatabaseHealthChecker
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseHealthChecker(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Database.CanConnectAsync(cancellationToken);
    }
}
