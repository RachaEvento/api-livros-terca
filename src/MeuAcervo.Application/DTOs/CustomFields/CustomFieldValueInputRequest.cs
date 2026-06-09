namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record CustomFieldValueInputRequest(
    string FieldKey,
    string? TextValue,
    decimal? NumberValue,
    DateTime? DateValue,
    bool? BooleanValue,
    string? OptionValue);
