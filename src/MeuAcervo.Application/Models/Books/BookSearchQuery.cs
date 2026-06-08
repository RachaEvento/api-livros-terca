namespace MeuAcervo.Application.Models.Books;

public sealed record BookSearchQuery(
    string? Search,
    string? Isbn,
    string? Title,
    string? Author,
    string? Language,
    int PageNumber,
    int PageSize);
