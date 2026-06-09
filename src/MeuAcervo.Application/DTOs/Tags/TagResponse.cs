namespace MeuAcervo.Application.DTOs.Tags;

public sealed record TagResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Color,
    string? Description,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
