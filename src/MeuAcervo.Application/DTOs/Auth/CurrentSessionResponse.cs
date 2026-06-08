using MeuAcervo.Application.DTOs.Identity;

namespace MeuAcervo.Application.DTOs.Auth;

public sealed record CurrentSessionResponse(
    TenantSummaryResponse Tenant,
    UserSummaryResponse User,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
