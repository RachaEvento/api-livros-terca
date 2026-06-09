using MeuAcervo.Domain.Common;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Domain.Entities;

public sealed class CustomFieldValue : TenantScopedAuditableEntityBase
{
    public CustomFieldEntityType EntityType { get; set; } = CustomFieldEntityType.UserLibraryItem;

    public Guid EntityId { get; set; }

    public Guid CustomFieldDefinitionId { get; set; }

    public string? TextValue { get; set; }

    public decimal? NumberValue { get; set; }

    public DateTime? DateValue { get; set; }

    public bool? BooleanValue { get; set; }

    public string? OptionValue { get; set; }

    public CustomFieldDefinition? CustomFieldDefinition { get; set; }
}
