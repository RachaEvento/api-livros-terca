namespace MeuAcervo.Application.DTOs.Catalog;

public sealed record BookWorkSummaryResponse(
    Guid Id,
    string CanonicalTitle,
    string? OriginalTitle,
    string? Description,
    int? FirstPublicationYear,
    string? PrimaryLanguage);
