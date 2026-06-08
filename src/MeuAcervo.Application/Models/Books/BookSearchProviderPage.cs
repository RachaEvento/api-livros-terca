using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Models.Books;

public sealed record BookSearchProviderPage(
    string Source,
    PagedResult<ExternalBookSearchResult> Results);
