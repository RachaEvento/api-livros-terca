using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Models.Profiles;

public sealed record PublicCustomFieldValueProjection(
    Guid LibraryItemId,
    Guid DefinitionId,
    string Label,
    CustomFieldDataType DataType,
    string? TextValue,
    decimal? NumberValue,
    DateTime? DateValue,
    bool? BooleanValue,
    string? OptionValue);
