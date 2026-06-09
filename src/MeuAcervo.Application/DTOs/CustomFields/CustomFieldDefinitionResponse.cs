using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record CustomFieldDefinitionResponse(
    Guid Id,
    CustomFieldEntityType EntityType,
    string Label,
    CustomFieldDataType DataType,
    bool IsPublic,
    bool IsActive,
    IReadOnlyCollection<CustomFieldOptionResponse> Options,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
