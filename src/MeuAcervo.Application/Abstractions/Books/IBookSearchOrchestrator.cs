using MeuAcervo.Application.Models.Books;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Abstractions.Books;

public interface IBookSearchOrchestrator
{
    Task<PagedResult<ExternalBookSearchResult>> SearchAsync(BookSearchQuery query, CancellationToken cancellationToken = default);
}
