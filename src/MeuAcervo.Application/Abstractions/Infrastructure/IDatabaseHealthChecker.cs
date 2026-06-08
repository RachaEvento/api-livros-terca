namespace MeuAcervo.Application.Abstractions.Infrastructure;

public interface IDatabaseHealthChecker
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
}
