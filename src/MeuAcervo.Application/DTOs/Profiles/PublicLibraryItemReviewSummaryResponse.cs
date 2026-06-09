namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record PublicLibraryItemReviewSummaryResponse(
    Guid ReviewId,
    int Rating,
    string? Title,
    bool ContainsSpoilers,
    DateTime? PublishedAtUtc);
