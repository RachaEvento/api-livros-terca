using MeuAcervo.Domain.Enums;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.DTOs.Profiles;

public sealed class GetPublicLibraryItemsRequest : PagedRequest
{
    public string? Search { get; init; }

    public string? Title { get; init; }

    public string? Author { get; init; }

    public ShelfType? ShelfType { get; init; }

    public ReadingStatus? ReadingStatus { get; init; }

    public bool? IsFavorite { get; init; }

    public AcquisitionFormat? AcquisitionFormat { get; init; }
}
