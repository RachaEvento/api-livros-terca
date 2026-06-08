namespace MeuAcervo.Application.DTOs.System;

public sealed record ApiInfoResponse(
    string Name,
    string Version,
    string Description,
    string Environment,
    string BaseRoute,
    bool JwtAuthenticationConfigured,
    string TenantResolutionStrategy,
    DateTime CurrentUtc);
