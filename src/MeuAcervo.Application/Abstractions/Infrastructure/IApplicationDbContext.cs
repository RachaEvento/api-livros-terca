namespace MeuAcervo.Application.Abstractions.Infrastructure;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
