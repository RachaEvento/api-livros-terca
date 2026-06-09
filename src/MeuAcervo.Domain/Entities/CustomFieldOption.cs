using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class CustomFieldOption : TenantScopedAuditableEntityBase
{
    public Guid CustomFieldDefinitionId { get; set; }

    public string Value { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public CustomFieldDefinition? CustomFieldDefinition { get; set; }
}
