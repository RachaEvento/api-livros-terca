namespace MeuAcervo.Application.DTOs.Identity;

public sealed record UserSummaryResponse(
    Guid Id,
    Guid TenantId,
    string Email,
    string Username,
    string DisplayName,
    bool IsActive);
