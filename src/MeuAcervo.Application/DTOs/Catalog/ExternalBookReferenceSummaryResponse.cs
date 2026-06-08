using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Catalog;

public sealed record ExternalBookReferenceSummaryResponse(
    Guid Id,
    string Provider,
    string ExternalId,
    ExternalBookReferenceType ReferenceType,
    string? ExternalUrl);
