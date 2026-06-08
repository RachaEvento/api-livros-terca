namespace MeuAcervo.Application.DTOs.Catalog;

public sealed record BookEditionSummaryResponse(
    Guid Id,
    Guid BookWorkId,
    string? Isbn10,
    string? Isbn13,
    string Title,
    string? Subtitle,
    Guid? PublisherId,
    DateTime? PublishedAt,
    int? PageCount,
    string Language,
    string? FormatDescriptor,
    string? CoverImageUrl,
    string? EditionNumber);
