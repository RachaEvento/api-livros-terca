using MeuAcervo.Domain.Common;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Domain.Entities;

public sealed class CustomFieldDefinition : TenantScopedSoftDeletableAuditableEntityBase
{
    public CustomFieldEntityType EntityType { get; set; } = CustomFieldEntityType.UserLibraryItem;

    public string Key { get; set; } = string.Empty;

    public string NormalizedKey { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public CustomFieldDataType DataType { get; set; } = CustomFieldDataType.Text;

    public bool IsRequired { get; set; }

    public bool IsPublic { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public string? ConfigurationJson { get; set; }

    public ICollection<CustomFieldOption> Options { get; set; } = new List<CustomFieldOption>();

    public ICollection<CustomFieldValue> Values { get; set; } = new List<CustomFieldValue>();
}
