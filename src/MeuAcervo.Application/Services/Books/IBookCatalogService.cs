using MeuAcervo.Application.DTOs.Books;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Services.Books;

public interface IBookCatalogService
{
    Task<PagedResult<BookSearchResultResponse>> SearchAsync(BookSearchRequest request, CancellationToken cancellationToken = default);

    Task<BookImportResponse> ImportAsync(BookImportRequest request, CancellationToken cancellationToken = default);
}
