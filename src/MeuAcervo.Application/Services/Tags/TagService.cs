using FluentValidation;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Abstractions.Library;
using MeuAcervo.Application.Abstractions.Tags;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Tags;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Shared.Text;

namespace MeuAcervo.Application.Services.Tags;

public sealed class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IUserLibraryRepository _userLibraryRepository;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IValidator<CreateTagRequest> _createTagRequestValidator;
    private readonly IValidator<UpdateTagRequest> _updateTagRequestValidator;
    private readonly IValidator<AssignLibraryItemTagsRequest> _assignLibraryItemTagsRequestValidator;

    public TagService(
        ITagRepository tagRepository,
        IUserLibraryRepository userLibraryRepository,
        IApplicationDbContext applicationDbContext,
        IValidator<CreateTagRequest> createTagRequestValidator,
        IValidator<UpdateTagRequest> updateTagRequestValidator,
        IValidator<AssignLibraryItemTagsRequest> assignLibraryItemTagsRequestValidator)
    {
        _tagRepository = tagRepository;
        _userLibraryRepository = userLibraryRepository;
        _applicationDbContext = applicationDbContext;
        _createTagRequestValidator = createTagRequestValidator;
        _updateTagRequestValidator = updateTagRequestValidator;
        _assignLibraryItemTagsRequestValidator = assignLibraryItemTagsRequestValidator;
    }

    public async Task<IReadOnlyCollection<TagResponse>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tags = await _tagRepository.GetAllAsync(tenantId, cancellationToken);
        return tags.Select(MapResponse).ToArray();
    }

    public async Task<TagResponse> CreateAsync(Guid tenantId, CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        await _createTagRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var slug = TextNormalizationHelper.Slugify(request.Name);
        if (await _tagRepository.SlugExistsAsync(tenantId, slug, null, cancellationToken))
        {
            throw new ConflictException("A tag with the same normalized name already exists for this tenant.");
        }

        var tag = new Tag
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Slug = slug,
            Color = TrimOrNull(request.Color),
            Description = TrimOrNull(request.Description)
        };

        _tagRepository.Add(tag);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return MapResponse(tag);
    }

    public async Task<TagResponse> UpdateAsync(Guid tenantId, Guid tagId, UpdateTagRequest request, CancellationToken cancellationToken = default)
    {
        await _updateTagRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var tag = await _tagRepository.GetByIdAsync(tenantId, tagId, tracking: true, cancellationToken)
                  ?? throw new NotFoundException("Tag was not found for the authenticated tenant.");

        var slug = TextNormalizationHelper.Slugify(request.Name);
        if (await _tagRepository.SlugExistsAsync(tenantId, slug, tagId, cancellationToken))
        {
            throw new ConflictException("A tag with the same normalized name already exists for this tenant.");
        }

        tag.Name = request.Name.Trim();
        tag.Slug = slug;
        tag.Color = TrimOrNull(request.Color);
        tag.Description = TrimOrNull(request.Description);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return MapResponse(tag);
    }

    public async Task DeleteAsync(Guid tenantId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var tag = await _tagRepository.GetByIdAsync(tenantId, tagId, tracking: true, cancellationToken)
                  ?? throw new NotFoundException("Tag was not found for the authenticated tenant.");

        _tagRepository.Remove(tag);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TagResponse>> AssignToLibraryItemAsync(Guid tenantId, Guid userId, Guid libraryItemId, AssignLibraryItemTagsRequest request, CancellationToken cancellationToken = default)
    {
        await _assignLibraryItemTagsRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var item = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, libraryItemId, cancellationToken)
                   ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        var tags = await _tagRepository.GetByIdsAsync(tenantId, request.TagIds, cancellationToken);
        if (tags.Count != request.TagIds.Distinct().Count())
        {
            throw new NotFoundException("One or more tags were not found for the authenticated tenant.");
        }

        var existingTagIds = item.UserLibraryItemTags.Select(link => link.TagId).ToHashSet();
        foreach (var tag in tags.Where(tag => !existingTagIds.Contains(tag.Id)))
        {
            var link = new UserLibraryItemTag
            {
                TenantId = tenantId,
                UserLibraryItemId = item.Id,
                TagId = tag.Id,
                UserLibraryItem = item,
                Tag = tag
            };

            _tagRepository.AddLibraryItemTag(link);
        }

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return item.UserLibraryItemTags
            .Select(link => link.Tag)
            .Where(tag => tag is not null)
            .Select(tag => MapResponse(tag!))
            .OrderBy(tag => tag.Name)
            .ToArray();
    }

    public async Task RemoveFromLibraryItemAsync(Guid tenantId, Guid userId, Guid libraryItemId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var item = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, libraryItemId, cancellationToken)
                   ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        var link = item.UserLibraryItemTags.FirstOrDefault(existingLink => existingLink.TagId == tagId)
                   ?? throw new NotFoundException("The tag is not associated with the informed library item.");

        _tagRepository.RemoveLibraryItemTag(link);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    private static TagResponse MapResponse(Tag tag)
    {
        return new TagResponse(tag.Id, tag.Name, tag.Slug, tag.Color, tag.Description, tag.CreatedAtUtc, tag.UpdatedAtUtc);
    }

    private static string? TrimOrNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
