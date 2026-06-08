using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Identity;

public sealed record TenantSummaryResponse(
    Guid Id,
    string Name,
    string Slug,
    TenantType Type);
