using MeuAcervo.Application.DTOs.Identity;

namespace MeuAcervo.Application.DTOs.Auth;

public sealed record AuthSessionResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    TenantSummaryResponse Tenant,
    UserSummaryResponse User,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
