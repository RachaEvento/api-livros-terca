namespace MeuAcervo.Application.Models.Profiles;

public sealed record PublicLibraryItemReviewSummaryProjection(
    Guid ReviewId,
    int Rating,
    string? Title,
    bool ContainsSpoilers,
    DateTime? PublishedAtUtc);
