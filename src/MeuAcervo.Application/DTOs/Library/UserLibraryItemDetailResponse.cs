namespace MeuAcervo.Application.DTOs.Library;

public sealed record UserLibraryItemDetailResponse(
    UserLibraryItemListResponse Item,
    IReadOnlyCollection<ReadingProgressEntryResponse> ProgressEntries);
