namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record CustomFieldOptionRequest(
    string Value,
    string Label,
    int SortOrder);
