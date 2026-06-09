using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.CustomFields;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;
using MeuAcervo.Infrastructure.Data;

namespace MeuAcervo.Infrastructure.Repositories;

public sealed class CustomFieldRepository : ICustomFieldRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CustomFieldRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CustomFieldDefinition>> GetDefinitionsAsync(Guid tenantId, CustomFieldEntityType entityType, bool includeInactive, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.CustomFieldDefinitions
            .Include(definition => definition.Options)
            .Where(definition => definition.TenantId == tenantId && definition.EntityType == entityType)
            .OrderBy(definition => definition.SortOrder)
            .ThenBy(definition => definition.Label)
            .AsSplitQuery()
            .AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(definition => definition.IsActive);
        }

        return await query.ToArrayAsync(cancellationToken);
    }

    public Task<CustomFieldDefinition?> GetDefinitionByIdAsync(Guid tenantId, Guid definitionId, bool tracking, CancellationToken cancellationToken = default)
    {
        var query = tracking
            ? _dbContext.CustomFieldDefinitions.AsQueryable()
            : _dbContext.CustomFieldDefinitions.AsNoTracking();

        return query
            .Include(definition => definition.Options)
            .AsSplitQuery()
            .FirstOrDefaultAsync(definition => definition.TenantId == tenantId && definition.Id == definitionId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, CustomFieldDefinition>> GetDefinitionsByIdsAsync(Guid tenantId, CustomFieldEntityType entityType, IReadOnlyCollection<Guid> definitionIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CustomFieldDefinitions
            .Include(definition => definition.Options)
            .Where(definition =>
                definition.TenantId == tenantId
                && definition.EntityType == entityType
                && definition.IsActive
                && definitionIds.Contains(definition.Id))
            .AsSplitQuery()
            .ToDictionaryAsync(definition => definition.Id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CustomFieldValue>> GetValuesAsync(Guid tenantId, CustomFieldEntityType entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CustomFieldValues
            .Include(value => value.CustomFieldDefinition)
                .ThenInclude(definition => definition!.Options)
            .Where(value => value.TenantId == tenantId && value.EntityType == entityType && value.EntityId == entityId)
            .AsSplitQuery()
            .ToArrayAsync(cancellationToken);
    }

    public async Task<int> GetNextDefinitionSortOrderAsync(Guid tenantId, CustomFieldEntityType entityType, CancellationToken cancellationToken = default)
    {
        var currentMax = await _dbContext.CustomFieldDefinitions
            .Where(definition => definition.TenantId == tenantId && definition.EntityType == entityType)
            .Select(definition => (int?)definition.SortOrder)
            .MaxAsync(cancellationToken);

        return (currentMax ?? 0) + 1;
    }

    public void AddDefinition(CustomFieldDefinition definition)
    {
        _dbContext.CustomFieldDefinitions.Add(definition);
    }

    public void RemoveDefinition(CustomFieldDefinition definition)
    {
        _dbContext.CustomFieldDefinitions.Remove(definition);
    }

    public void AddOption(CustomFieldOption option)
    {
        _dbContext.CustomFieldOptions.Add(option);
    }

    public void AddValue(CustomFieldValue value)
    {
        _dbContext.CustomFieldValues.Add(value);
    }

    public void RemoveValue(CustomFieldValue value)
    {
        _dbContext.CustomFieldValues.Remove(value);
    }
}
