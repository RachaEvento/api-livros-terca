using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Library;

public sealed record GetUserLibraryItemsRequest(
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
    string? SortBy = null,
    string? SortDirection = null,
    int PageNumber = 1,
    int PageSize = 20);
