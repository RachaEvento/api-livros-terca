using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Reviews;

public sealed record ReviewResponse(
    Guid Id,
    int Rating,
    string? Title,
    string Content,
    ReviewVisibility Visibility,
    bool ContainsSpoilers,
    DateTime? PublishedAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
