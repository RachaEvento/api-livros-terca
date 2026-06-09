using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record PublicLibraryItemResponse(
    Guid LibraryItemId,
    Guid BookEditionId,
    Guid BookWorkId,
    string WorkTitle,
    string EditionTitle,
    string? Subtitle,
    IReadOnlyCollection<string> Authors,
    string? Publisher,
    string Language,
    string? CoverImageUrl,
    ShelfType ShelfType,
    ReadingStatus ReadingStatus,
    AcquisitionFormat? AcquisitionFormat,
    bool IsFavorite,
    decimal? ProgressPercent,
    int ReadCount,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    DateTime? AcquiredAt,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    PublicLibraryItemReviewSummaryResponse? Review,
    IReadOnlyCollection<PublicCustomFieldValueResponse> CustomFields);
