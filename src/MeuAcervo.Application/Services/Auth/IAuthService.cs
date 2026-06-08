using MeuAcervo.Application.DTOs.Auth;

namespace MeuAcervo.Application.Services.Auth;

public interface IAuthService
{
    Task<AuthSessionResponse> RegisterAsync(RegisterRequest request, string? clientIpAddress, CancellationToken cancellationToken = default);

    Task<AuthSessionResponse> LoginAsync(LoginRequest request, string? clientIpAddress, CancellationToken cancellationToken = default);

    Task<AuthSessionResponse> RefreshAsync(RefreshTokenRequest request, string? clientIpAddress, CancellationToken cancellationToken = default);

    Task<LogoutResponse> LogoutAsync(Guid authenticatedUserId, Guid authenticatedTenantId, LogoutRequest request, CancellationToken cancellationToken = default);

    Task<CurrentSessionResponse> GetCurrentSessionAsync(Guid authenticatedUserId, Guid authenticatedTenantId, CancellationToken cancellationToken = default);
}
