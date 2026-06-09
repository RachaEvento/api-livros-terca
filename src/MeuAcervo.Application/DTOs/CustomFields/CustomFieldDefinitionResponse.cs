using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record CustomFieldDefinitionResponse(
    Guid Id,
    CustomFieldEntityType EntityType,
    string Key,
    string Label,
    CustomFieldDataType DataType,
    bool IsRequired,
    bool IsPublic,
    bool IsActive,
    int SortOrder,
    string? ConfigurationJson,
    IReadOnlyCollection<CustomFieldOptionResponse> Options,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
