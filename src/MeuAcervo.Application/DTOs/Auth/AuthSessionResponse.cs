using MeuAcervo.Application.DTOs.Identity;

namespace MeuAcervo.Application.DTOs.Auth;

public sealed record AuthSessionResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    TenantSummaryResponse Tenant,
    UserSummaryResponse User,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
