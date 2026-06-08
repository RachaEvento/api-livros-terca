using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Library;

public sealed record UserLibraryItemListResponse(
    Guid Id,
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
    OwnershipType? OwnershipType,
    bool IsFavorite,
    int? CurrentPage,
    decimal? ProgressPercent,
    int ReadCount,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    DateTime? AcquiredAt,
    string? PhysicalLocation,
    string? Condition,
    string? PrivateNotes,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
