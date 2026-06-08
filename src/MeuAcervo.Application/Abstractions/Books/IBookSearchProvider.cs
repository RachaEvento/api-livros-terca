using MeuAcervo.Application.Models.Books;

namespace MeuAcervo.Application.Abstractions.Books;

public interface IBookSearchProvider
{
    string Source { get; }

    Task<BookSearchProviderPage> SearchAsync(BookSearchQuery query, CancellationToken cancellationToken = default);
}
