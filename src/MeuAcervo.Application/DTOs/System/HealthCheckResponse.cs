namespace MeuAcervo.Application.DTOs.System;

public sealed record HealthCheckResponse(
    string Status,
    bool DatabaseAvailable,
    DateTime CheckedAtUtc);
