using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Reviews;

public sealed record UpsertReviewRequest(
    int Rating,
    string? Title,
    string Content,
    ReviewVisibility Visibility,
    bool ContainsSpoilers);
