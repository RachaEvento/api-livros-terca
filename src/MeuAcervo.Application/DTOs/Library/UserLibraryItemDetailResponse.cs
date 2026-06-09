using MeuAcervo.Application.DTOs.CustomFields;
using MeuAcervo.Application.DTOs.Reviews;

namespace MeuAcervo.Application.DTOs.Library;

public sealed record UserLibraryItemDetailResponse(
    UserLibraryItemListResponse Item,
    IReadOnlyCollection<ReadingProgressEntryResponse> ProgressEntries,
    IReadOnlyCollection<UserLibraryItemTagResponse> Tags,
    IReadOnlyCollection<CustomFieldValueResponse> CustomFields,
    ReviewResponse? Review,
    IReadOnlyCollection<UserLibraryItemLoanSummaryResponse> Loans);
