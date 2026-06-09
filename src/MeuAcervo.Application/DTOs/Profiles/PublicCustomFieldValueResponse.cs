using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record PublicCustomFieldValueResponse(
    Guid DefinitionId,
    string Label,
    CustomFieldDataType DataType,
    string? TextValue,
    decimal? NumberValue,
    DateTime? DateValue,
    bool? BooleanValue,
    string? OptionValue);
