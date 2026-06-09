using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record PublicFavoriteItemResponse(
    Guid LibraryItemId,
    Guid BookEditionId,
    Guid BookWorkId,
    ShelfType ShelfType,
    ReadingStatus ReadingStatus,
    decimal? ProgressPercent,
    int ReadCount,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    DateTime UpdatedAtUtc,
    string Title,
    string? Subtitle,
    string CanonicalTitle,
    string Language,
    string? CoverImageUrl,
    string? Isbn13,
    DateTime? PublishedAt,
    string? PublisherName,
    IReadOnlyCollection<string> Authors,
    IReadOnlyCollection<PublicCustomFieldValueResponse> CustomFields);
