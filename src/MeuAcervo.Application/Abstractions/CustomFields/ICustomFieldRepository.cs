using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Abstractions.CustomFields;

public interface ICustomFieldRepository
{
    Task<IReadOnlyCollection<CustomFieldDefinition>> GetDefinitionsAsync(Guid tenantId, CustomFieldEntityType entityType, bool includeInactive, CancellationToken cancellationToken = default);

    Task<CustomFieldDefinition?> GetDefinitionByIdAsync(Guid tenantId, Guid definitionId, bool tracking, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, CustomFieldDefinition>> GetDefinitionsByIdsAsync(Guid tenantId, CustomFieldEntityType entityType, IReadOnlyCollection<Guid> definitionIds, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CustomFieldValue>> GetValuesAsync(Guid tenantId, CustomFieldEntityType entityType, Guid entityId, CancellationToken cancellationToken = default);

    Task<int> GetNextDefinitionSortOrderAsync(Guid tenantId, CustomFieldEntityType entityType, CancellationToken cancellationToken = default);

    void AddDefinition(CustomFieldDefinition definition);

    void RemoveDefinition(CustomFieldDefinition definition);

    void AddOption(CustomFieldOption option);

    void AddValue(CustomFieldValue value);

    void RemoveValue(CustomFieldValue value);
}
