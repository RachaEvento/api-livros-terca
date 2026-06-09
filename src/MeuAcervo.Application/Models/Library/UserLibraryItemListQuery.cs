using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Models.Library;

public sealed record UserLibraryItemListQuery(
    string? Search,
    string? Title,
    string? Author,
    Guid? TagId,
    ShelfType? ShelfType,
    ReadingStatus? ReadingStatus,
    bool? IsFavorite,
    AcquisitionFormat? AcquisitionFormat,
    DateTime? UpdatedFrom,
    DateTime? UpdatedTo,
    string SortBy,
    string SortDirection,
    int PageNumber,
    int PageSize);
