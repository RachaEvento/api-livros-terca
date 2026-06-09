using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record CreateCustomFieldDefinitionRequest
{
    public CustomFieldEntityType EntityType { get; init; }

    public string Label { get; init; } = string.Empty;

    public CustomFieldDataType DataType { get; init; }

    public bool IsPublic { get; init; }

    public bool IsActive { get; init; }

    public IReadOnlyCollection<CustomFieldOptionRequest> Options { get; init; } = [];
}
