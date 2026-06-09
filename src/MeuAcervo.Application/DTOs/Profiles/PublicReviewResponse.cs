namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record PublicReviewResponse(
    Guid ReviewId,
    int Rating,
    string? Title,
    string Content,
    bool ContainsSpoilers,
    DateTime? PublishedAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    Guid LibraryItemId,
    Guid BookEditionId,
    Guid BookWorkId,
    string BookTitle,
    string? BookSubtitle,
    string CanonicalTitle,
    string? CoverImageUrl,
    string? PublisherName,
    IReadOnlyCollection<string> Authors);
