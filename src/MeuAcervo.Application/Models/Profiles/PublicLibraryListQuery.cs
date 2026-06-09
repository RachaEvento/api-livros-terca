using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Models.Profiles;

public sealed record PublicLibraryListQuery(
    string? Search,
    string? Title,
    string? Author,
    ShelfType? ShelfType,
    ReadingStatus? ReadingStatus,
    bool? IsFavorite,
    AcquisitionFormat? AcquisitionFormat,
    string SortBy,
    string SortDirection,
    int PageNumber,
    int PageSize);
