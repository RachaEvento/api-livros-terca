namespace MeuAcervo.Application.DTOs.Books;

public sealed record BookImportRequest(
    string Source,
    string? ExternalId,
    string? WorkExternalId,
    string Title,
    string? WorkTitle,
    string? Subtitle,
    IReadOnlyCollection<string> Authors,
    string? Isbn10,
    string? Isbn13,
    string? Publisher,
    int? PublishedYear,
    int? FirstPublishedYear,
    string? Language,
    int? PageCount,
    string? CoverImageUrl,
    string? ExternalUrl,
    string? Description);
