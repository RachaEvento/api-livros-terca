using MeuAcervo.Application.DTOs.Books;
using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Abstractions.Books;

public interface IBookEditionMatcher
{
    Task<BookEdition?> FindMatchingEditionAsync(BookImportRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookEdition?>> FindMatchingEditionsAsync(
        IReadOnlyList<BookImportRequest> requests,
        CancellationToken cancellationToken = default);
}
