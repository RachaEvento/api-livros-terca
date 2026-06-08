using MeuAcervo.Application.DTOs.Library;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Services.Library;

public interface IUserLibraryService
{
    Task<PagedResult<UserLibraryItemListResponse>> GetItemsAsync(Guid tenantId, Guid userId, GetUserLibraryItemsRequest request, CancellationToken cancellationToken = default);

    Task<UserLibraryItemDetailResponse> CreateItemAsync(Guid tenantId, Guid userId, CreateUserLibraryItemRequest request, CancellationToken cancellationToken = default);

    Task<UserLibraryItemDetailResponse> GetItemByIdAsync(Guid tenantId, Guid userId, Guid itemId, CancellationToken cancellationToken = default);

    Task<UserLibraryItemDetailResponse> UpdateItemAsync(Guid tenantId, Guid userId, Guid itemId, UpdateUserLibraryItemRequest request, CancellationToken cancellationToken = default);

    Task<UserLibraryItemDetailResponse> UpdateStatusAsync(Guid tenantId, Guid userId, Guid itemId, UpdateUserLibraryItemStatusRequest request, CancellationToken cancellationToken = default);

    Task<UserLibraryItemDetailResponse> RegisterProgressAsync(Guid tenantId, Guid userId, Guid itemId, RegisterReadingProgressRequest request, CancellationToken cancellationToken = default);

    Task DeleteItemAsync(Guid tenantId, Guid userId, Guid itemId, CancellationToken cancellationToken = default);
}
