using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Library;

public sealed record CreateUserLibraryItemRequest(
    Guid BookEditionId,
    ShelfType ShelfType,
    ReadingStatus? ReadingStatus,
    AcquisitionFormat? AcquisitionFormat,
    OwnershipType? OwnershipType,
    bool IsFavorite,
    int? CurrentPage,
    decimal? ProgressPercent,
    int? ReadCount,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    DateTime? AcquiredAt,
    string? PhysicalLocation,
    string? Condition,
    string? PrivateNotes);
