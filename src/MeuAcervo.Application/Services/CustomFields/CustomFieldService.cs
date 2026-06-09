using FluentValidation;
using MeuAcervo.Application.Abstractions.CustomFields;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Abstractions.Library;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.CustomFields;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;
using MeuAcervo.Shared.Text;

namespace MeuAcervo.Application.Services.CustomFields;

public sealed class CustomFieldService : ICustomFieldService
{
    private readonly ICustomFieldRepository _customFieldRepository;
    private readonly IUserLibraryRepository _userLibraryRepository;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IValidator<GetCustomFieldDefinitionsRequest> _getCustomFieldDefinitionsRequestValidator;
    private readonly IValidator<CreateCustomFieldDefinitionRequest> _createCustomFieldDefinitionRequestValidator;
    private readonly IValidator<UpdateCustomFieldDefinitionRequest> _updateCustomFieldDefinitionRequestValidator;
    private readonly IValidator<UpsertLibraryItemCustomFieldValuesRequest> _upsertLibraryItemCustomFieldValuesRequestValidator;

    public CustomFieldService(
        ICustomFieldRepository customFieldRepository,
        IUserLibraryRepository userLibraryRepository,
        IApplicationDbContext applicationDbContext,
        IValidator<GetCustomFieldDefinitionsRequest> getCustomFieldDefinitionsRequestValidator,
        IValidator<CreateCustomFieldDefinitionRequest> createCustomFieldDefinitionRequestValidator,
        IValidator<UpdateCustomFieldDefinitionRequest> updateCustomFieldDefinitionRequestValidator,
        IValidator<UpsertLibraryItemCustomFieldValuesRequest> upsertLibraryItemCustomFieldValuesRequestValidator)
    {
        _customFieldRepository = customFieldRepository;
        _userLibraryRepository = userLibraryRepository;
        _applicationDbContext = applicationDbContext;
        _getCustomFieldDefinitionsRequestValidator = getCustomFieldDefinitionsRequestValidator;
        _createCustomFieldDefinitionRequestValidator = createCustomFieldDefinitionRequestValidator;
        _updateCustomFieldDefinitionRequestValidator = updateCustomFieldDefinitionRequestValidator;
        _upsertLibraryItemCustomFieldValuesRequestValidator = upsertLibraryItemCustomFieldValuesRequestValidator;
    }

    public async Task<IReadOnlyCollection<CustomFieldDefinitionResponse>> GetDefinitionsAsync(Guid tenantId, GetCustomFieldDefinitionsRequest request, CancellationToken cancellationToken = default)
    {
        await _getCustomFieldDefinitionsRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var entityType = request.EntityType ?? CustomFieldEntityType.UserLibraryItem;
        var definitions = await _customFieldRepository.GetDefinitionsAsync(tenantId, entityType, request.IncludeInactive, cancellationToken);
        return definitions.Select(MapDefinitionResponse).ToArray();
    }

    public async Task<CustomFieldDefinitionResponse> CreateDefinitionAsync(Guid tenantId, CreateCustomFieldDefinitionRequest request, CancellationToken cancellationToken = default)
    {
        await _createCustomFieldDefinitionRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var normalizedKey = NormalizeKey(request.Key);
        if (await _customFieldRepository.NormalizedKeyExistsAsync(tenantId, request.EntityType, normalizedKey, null, cancellationToken))
        {
            throw new ConflictException("A custom field with the same normalized key already exists for this entity type.");
        }

        var definition = new CustomFieldDefinition
        {
            TenantId = tenantId,
            EntityType = request.EntityType,
            Key = request.Key.Trim(),
            NormalizedKey = normalizedKey,
            Label = request.Label.Trim(),
            DataType = request.DataType,
            IsRequired = request.IsRequired,
            IsPublic = request.IsPublic,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            ConfigurationJson = TrimOrNull(request.ConfigurationJson)
        };

        SyncOptions(definition, request.Options);

        _customFieldRepository.AddDefinition(definition);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return MapDefinitionResponse(definition);
    }

    public async Task<CustomFieldDefinitionResponse> UpdateDefinitionAsync(Guid tenantId, Guid definitionId, UpdateCustomFieldDefinitionRequest request, CancellationToken cancellationToken = default)
    {
        await _updateCustomFieldDefinitionRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var definition = await _customFieldRepository.GetDefinitionByIdAsync(tenantId, definitionId, tracking: true, cancellationToken)
                         ?? throw new NotFoundException("Custom field definition was not found for the authenticated tenant.");

        if (definition.DataType != request.DataType)
        {
            throw new ConflictException("Changing the data type of an existing custom field definition is not supported.");
        }

        definition.Label = request.Label.Trim();
        definition.IsRequired = request.IsRequired;
        definition.IsPublic = request.IsPublic;
        definition.IsActive = request.IsActive;
        definition.SortOrder = request.SortOrder;
        definition.ConfigurationJson = TrimOrNull(request.ConfigurationJson);

        SyncOptions(definition, request.Options);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return MapDefinitionResponse(definition);
    }

    public async Task DeleteDefinitionAsync(Guid tenantId, Guid definitionId, CancellationToken cancellationToken = default)
    {
        var definition = await _customFieldRepository.GetDefinitionByIdAsync(tenantId, definitionId, tracking: true, cancellationToken)
                         ?? throw new NotFoundException("Custom field definition was not found for the authenticated tenant.");

        _customFieldRepository.RemoveDefinition(definition);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<CustomFieldValueResponse>> UpsertLibraryItemValuesAsync(Guid tenantId, Guid userId, Guid libraryItemId, UpsertLibraryItemCustomFieldValuesRequest request, CancellationToken cancellationToken = default)
    {
        await _upsertLibraryItemCustomFieldValuesRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var libraryItem = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, libraryItemId, cancellationToken)
                          ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        var normalizedKeys = request.Values.Select(value => NormalizeKey(value.FieldKey)).ToArray();
        var definitionsByKey = await _customFieldRepository.GetDefinitionsByNormalizedKeysAsync(
            tenantId,
            CustomFieldEntityType.UserLibraryItem,
            normalizedKeys,
            cancellationToken);

        if (definitionsByKey.Count != normalizedKeys.Length)
        {
            throw new NotFoundException("One or more custom field keys were not found for the authenticated tenant.");
        }

        var activeDefinitions = await _customFieldRepository.GetDefinitionsAsync(tenantId, CustomFieldEntityType.UserLibraryItem, includeInactive: false, cancellationToken);
        var requiredDefinitionKeys = activeDefinitions
            .Where(definition => definition.IsRequired)
            .Select(definition => definition.NormalizedKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (requiredDefinitionKeys.Any(requiredKey => !definitionsByKey.ContainsKey(requiredKey)))
        {
            throw new BusinessRuleException("All active required custom fields must be provided in the request.");
        }

        var existingValues = await _customFieldRepository.GetValuesAsync(tenantId, CustomFieldEntityType.UserLibraryItem, libraryItem.Id, cancellationToken);
        var existingValuesByDefinitionId = existingValues.ToDictionary(value => value.CustomFieldDefinitionId);

        foreach (var existingValue in existingValues)
        {
            _customFieldRepository.RemoveValue(existingValue);
        }

        foreach (var input in request.Values)
        {
            var normalizedKey = NormalizeKey(input.FieldKey);
            var definition = definitionsByKey[normalizedKey];

            ValidateValueAgainstDefinition(definition, input);

            var value = new CustomFieldValue
            {
                TenantId = tenantId,
                EntityType = CustomFieldEntityType.UserLibraryItem,
                EntityId = libraryItem.Id,
                CustomFieldDefinitionId = definition.Id,
                TextValue = TrimOrNull(input.TextValue),
                NumberValue = input.NumberValue,
                DateValue = input.DateValue?.ToUniversalTime(),
                BooleanValue = input.BooleanValue,
                OptionValue = TrimOrNull(input.OptionValue)
            };

            _customFieldRepository.AddValue(value);
        }

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return await GetLibraryItemValuesAsync(tenantId, libraryItemId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CustomFieldValueResponse>> GetLibraryItemValuesAsync(Guid tenantId, Guid libraryItemId, CancellationToken cancellationToken = default)
    {
        var values = await _customFieldRepository.GetValuesAsync(tenantId, CustomFieldEntityType.UserLibraryItem, libraryItemId, cancellationToken);
        return values
            .OrderBy(value => value.CustomFieldDefinition?.SortOrder ?? int.MaxValue)
            .ThenBy(value => value.CustomFieldDefinition?.Label)
            .Select(MapValueResponse)
            .ToArray();
    }

    private static void SyncOptions(CustomFieldDefinition definition, IReadOnlyCollection<CustomFieldOptionRequest> optionRequests)
    {
        definition.Options.Clear();

        foreach (var optionRequest in optionRequests
                     .OrderBy(option => option.SortOrder)
                     .ThenBy(option => option.Label, StringComparer.OrdinalIgnoreCase))
        {
            definition.Options.Add(new CustomFieldOption
            {
                TenantId = definition.TenantId,
                CustomFieldDefinitionId = definition.Id,
                Value = optionRequest.Value.Trim(),
                Label = optionRequest.Label.Trim(),
                SortOrder = optionRequest.SortOrder
            });
        }
    }

    private static void ValidateValueAgainstDefinition(CustomFieldDefinition definition, CustomFieldValueInputRequest input)
    {
        switch (definition.DataType)
        {
            case CustomFieldDataType.Text when input.TextValue is null:
            case CustomFieldDataType.Number when !input.NumberValue.HasValue:
            case CustomFieldDataType.Date when !input.DateValue.HasValue:
            case CustomFieldDataType.Boolean when !input.BooleanValue.HasValue:
            case CustomFieldDataType.List when string.IsNullOrWhiteSpace(input.OptionValue):
                throw new BusinessRuleException($"Custom field '{definition.Key}' expects a value compatible with {definition.DataType}.");
        }

        if (definition.DataType == CustomFieldDataType.List)
        {
            var allowedOptions = definition.Options.Select(option => option.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!allowedOptions.Contains(input.OptionValue!.Trim()))
            {
                throw new BusinessRuleException($"The option informed for custom field '{definition.Key}' does not exist.");
            }
        }
    }

    private static CustomFieldDefinitionResponse MapDefinitionResponse(CustomFieldDefinition definition)
    {
        return new CustomFieldDefinitionResponse(
            definition.Id,
            definition.EntityType,
            definition.Key,
            definition.Label,
            definition.DataType,
            definition.IsRequired,
            definition.IsPublic,
            definition.IsActive,
            definition.SortOrder,
            definition.ConfigurationJson,
            definition.Options
                .OrderBy(option => option.SortOrder)
                .ThenBy(option => option.Label)
                .Select(option => new CustomFieldOptionResponse(option.Id, option.Value, option.Label, option.SortOrder))
                .ToArray(),
            definition.CreatedAtUtc,
            definition.UpdatedAtUtc);
    }

    private static CustomFieldValueResponse MapValueResponse(CustomFieldValue value)
    {
        var definition = value.CustomFieldDefinition!;
        return new CustomFieldValueResponse(
            definition.Id,
            definition.Key,
            definition.Label,
            definition.DataType,
            definition.IsPublic,
            value.TextValue,
            value.NumberValue,
            value.DateValue,
            value.BooleanValue,
            value.OptionValue);
    }

    private static string NormalizeKey(string key)
    {
        return TextNormalizationHelper.Slugify(key);
    }

    private static string? TrimOrNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
