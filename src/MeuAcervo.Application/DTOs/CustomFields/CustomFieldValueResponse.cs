using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record CustomFieldValueResponse(
    Guid DefinitionId,
    string Label,
    CustomFieldDataType DataType,
    bool IsPublic,
    string? TextValue,
    decimal? NumberValue,
    DateTime? DateValue,
    bool? BooleanValue,
    string? OptionValue);
