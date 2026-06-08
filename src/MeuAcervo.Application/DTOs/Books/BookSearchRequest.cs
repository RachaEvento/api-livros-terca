namespace MeuAcervo.Application.DTOs.Books;

public sealed record BookSearchRequest(
    string? Search,
    string? Isbn,
    string? Title,
    string? Author,
    string? Language,
    int PageNumber = 1,
    int PageSize = 20);
