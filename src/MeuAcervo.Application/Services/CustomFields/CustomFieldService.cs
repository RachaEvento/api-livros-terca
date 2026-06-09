using FluentValidation;
using MeuAcervo.Application.Abstractions.CustomFields;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Abstractions.Library;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.CustomFields;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;

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

        var definition = new CustomFieldDefinition
        {
            TenantId = tenantId,
            EntityType = request.EntityType,
            Label = request.Label.Trim(),
            DataType = request.DataType,
            IsPublic = request.IsPublic,
            IsActive = request.IsActive,
            SortOrder = await _customFieldRepository.GetNextDefinitionSortOrderAsync(tenantId, request.EntityType, cancellationToken)
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
        definition.IsPublic = request.IsPublic;
        definition.IsActive = request.IsActive;

        SyncOptions(definition, request.Options, _customFieldRepository.AddOption);

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

        var definitionIds = request.Values.Select(value => value.DefinitionId).ToArray();
        var definitionsById = await _customFieldRepository.GetDefinitionsByIdsAsync(
            tenantId,
            CustomFieldEntityType.UserLibraryItem,
            definitionIds,
            cancellationToken);

        if (definitionsById.Count != definitionIds.Length)
        {
            throw new NotFoundException("One or more custom field definitions were not found for the authenticated tenant.");
        }

        var existingValues = await _customFieldRepository.GetValuesAsync(tenantId, CustomFieldEntityType.UserLibraryItem, libraryItem.Id, cancellationToken);

        foreach (var existingValue in existingValues)
        {
            _customFieldRepository.RemoveValue(existingValue);
        }

        foreach (var input in request.Values)
        {
            var definition = definitionsById[input.DefinitionId];

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

    private static void SyncOptions(
        CustomFieldDefinition definition,
        IReadOnlyCollection<CustomFieldOptionRequest> optionRequests,
        Action<CustomFieldOption>? registerNewOption = null)
    {
        definition.Options.Clear();

        var sortOrder = 1;

        foreach (var optionRequest in optionRequests)
        {
            var option = new CustomFieldOption
            {
                TenantId = definition.TenantId,
                CustomFieldDefinitionId = definition.Id,
                Value = optionRequest.Value.Trim(),
                Label = optionRequest.Label.Trim(),
                SortOrder = sortOrder++
            };

            definition.Options.Add(option);

            // Existing tracked definitions need new options explicitly marked as Added,
            // otherwise EF treats GUID-backed dependents as updates and SaveChanges fails.
            registerNewOption?.Invoke(option);
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
                throw new BusinessRuleException($"Custom field '{definition.Label}' expects a value compatible with {definition.DataType}.");
        }

        if (definition.DataType == CustomFieldDataType.List)
        {
            var allowedOptions = definition.Options.Select(option => option.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!allowedOptions.Contains(input.OptionValue!.Trim()))
            {
                throw new BusinessRuleException($"The option informed for custom field '{definition.Label}' does not exist.");
            }
        }
    }

    private static CustomFieldDefinitionResponse MapDefinitionResponse(CustomFieldDefinition definition)
    {
        return new CustomFieldDefinitionResponse(
            definition.Id,
            definition.EntityType,
            definition.Label,
            definition.DataType,
            definition.IsPublic,
            definition.IsActive,
            definition.Options
                .OrderBy(option => option.SortOrder)
                .ThenBy(option => option.Label)
                .Select(option => new CustomFieldOptionResponse(option.Id, option.Value, option.Label))
                .ToArray(),
            definition.CreatedAtUtc,
            definition.UpdatedAtUtc);
    }

    private static CustomFieldValueResponse MapValueResponse(CustomFieldValue value)
    {
        var definition = value.CustomFieldDefinition!;
        return new CustomFieldValueResponse(
            definition.Id,
            definition.Label,
            definition.DataType,
            definition.IsPublic,
            value.TextValue,
            value.NumberValue,
            value.DateValue,
            value.BooleanValue,
            value.OptionValue);
    }

    private static string? TrimOrNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

}
