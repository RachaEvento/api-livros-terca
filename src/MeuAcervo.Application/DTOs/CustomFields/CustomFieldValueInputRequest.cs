namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record CustomFieldValueInputRequest(
    Guid DefinitionId,
    string? TextValue,
    decimal? NumberValue,
    DateTime? DateValue,
    bool? BooleanValue,
    string? OptionValue);
