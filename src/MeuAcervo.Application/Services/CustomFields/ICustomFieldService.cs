using MeuAcervo.Application.DTOs.CustomFields;

namespace MeuAcervo.Application.Services.CustomFields;

public interface ICustomFieldService
{
    Task<IReadOnlyCollection<CustomFieldDefinitionResponse>> GetDefinitionsAsync(Guid tenantId, GetCustomFieldDefinitionsRequest request, CancellationToken cancellationToken = default);

    Task<CustomFieldDefinitionResponse> CreateDefinitionAsync(Guid tenantId, CreateCustomFieldDefinitionRequest request, CancellationToken cancellationToken = default);

    Task<CustomFieldDefinitionResponse> UpdateDefinitionAsync(Guid tenantId, Guid definitionId, UpdateCustomFieldDefinitionRequest request, CancellationToken cancellationToken = default);

    Task DeleteDefinitionAsync(Guid tenantId, Guid definitionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CustomFieldValueResponse>> UpsertLibraryItemValuesAsync(Guid tenantId, Guid userId, Guid libraryItemId, UpsertLibraryItemCustomFieldValuesRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CustomFieldValueResponse>> GetLibraryItemValuesAsync(Guid tenantId, Guid libraryItemId, CancellationToken cancellationToken = default);
}
