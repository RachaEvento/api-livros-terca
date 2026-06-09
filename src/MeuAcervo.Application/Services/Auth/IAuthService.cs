using MeuAcervo.Application.DTOs.Auth;

namespace MeuAcervo.Application.Services.Auth;

public interface IAuthService
{
    Task<AuthSessionResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthSessionResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<LogoutResponse> LogoutAsync(CancellationToken cancellationToken = default);

    Task<CurrentSessionResponse> GetCurrentSessionAsync(Guid authenticatedUserId, Guid authenticatedTenantId, CancellationToken cancellationToken = default);
}
