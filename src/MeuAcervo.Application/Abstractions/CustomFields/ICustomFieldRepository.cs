using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Abstractions.CustomFields;

public interface ICustomFieldRepository
{
    Task<IReadOnlyCollection<CustomFieldDefinition>> GetDefinitionsAsync(Guid tenantId, CustomFieldEntityType entityType, bool includeInactive, CancellationToken cancellationToken = default);

    Task<CustomFieldDefinition?> GetDefinitionByIdAsync(Guid tenantId, Guid definitionId, bool tracking, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, CustomFieldDefinition>> GetDefinitionsByNormalizedKeysAsync(Guid tenantId, CustomFieldEntityType entityType, IReadOnlyCollection<string> normalizedKeys, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CustomFieldValue>> GetValuesAsync(Guid tenantId, CustomFieldEntityType entityType, Guid entityId, CancellationToken cancellationToken = default);

    Task<bool> NormalizedKeyExistsAsync(Guid tenantId, CustomFieldEntityType entityType, string normalizedKey, Guid? excludingDefinitionId, CancellationToken cancellationToken = default);

    void AddDefinition(CustomFieldDefinition definition);

    void RemoveDefinition(CustomFieldDefinition definition);

    void AddValue(CustomFieldValue value);

    void RemoveValue(CustomFieldValue value);
}
