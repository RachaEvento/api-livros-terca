using MeuAcervo.Application.DTOs.Tags;

namespace MeuAcervo.Application.Services.Tags;

public interface ITagService
{
    Task<IReadOnlyCollection<TagResponse>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<TagResponse> CreateAsync(Guid tenantId, CreateTagRequest request, CancellationToken cancellationToken = default);

    Task<TagResponse> UpdateAsync(Guid tenantId, Guid tagId, UpdateTagRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid tenantId, Guid tagId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TagResponse>> AssignToLibraryItemAsync(Guid tenantId, Guid userId, Guid libraryItemId, AssignLibraryItemTagsRequest request, CancellationToken cancellationToken = default);

    Task RemoveFromLibraryItemAsync(Guid tenantId, Guid userId, Guid libraryItemId, Guid tagId, CancellationToken cancellationToken = default);
}
