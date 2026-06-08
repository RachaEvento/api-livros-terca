using MeuAcervo.Application.DTOs.Catalog;

namespace MeuAcervo.Application.DTOs.Books;

public sealed record BookImportResponse(
    bool CreatedNewWork,
    bool CreatedNewEdition,
    BookWorkSummaryResponse Work,
    BookEditionSummaryResponse Edition,
    PublisherSummaryResponse? Publisher,
    IReadOnlyCollection<AuthorSummaryResponse> Authors,
    IReadOnlyCollection<ExternalBookReferenceSummaryResponse> ExternalReferences);
