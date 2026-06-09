using MeuAcervo.Application.DTOs.Profiles;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Services.Profiles;

public interface IUserProfileService
{
    Task<UserProfileSettingsResponse> GetCurrentAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);

    Task<UserProfileSettingsResponse> UpdateCurrentAsync(Guid tenantId, Guid userId, UpdateUserProfileSettingsRequest request, CancellationToken cancellationToken = default);

    Task<PublicUserProfileResponse> GetPublicProfileAsync(string username, CancellationToken cancellationToken = default);

    Task<PagedResult<PublicLibraryItemResponse>> GetPublicLibraryAsync(string username, GetPublicLibraryItemsRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<PublicFavoriteItemResponse>> GetPublicFavoritesAsync(string username, GetPublicFavoritesRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<PublicReviewResponse>> GetPublicReviewsAsync(string username, GetPublicReviewsRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<PublicActivityEntryResponse>> GetPublicActivityAsync(string username, GetPublicActivityRequest request, CancellationToken cancellationToken = default);
}
