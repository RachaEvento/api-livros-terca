namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record CustomFieldOptionResponse(
    Guid Id,
    string Value,
    string Label,
    int SortOrder);
