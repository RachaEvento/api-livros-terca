using MeuAcervo.Application.Models.Profiles;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Abstractions.Profiles;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid tenantId, Guid userId, bool tracking, CancellationToken cancellationToken = default);

    void Add(UserProfile profile);

    Task<PublicProfileLookup?> GetPublicProfileLookupAsync(string normalizedUsername, CancellationToken cancellationToken = default);

    Task<PublicProfileStatisticsProjection> GetPublicStatisticsAsync(Guid tenantId, Guid userId, bool includeWishlist, CancellationToken cancellationToken = default);

    Task<PagedResult<PublicLibraryItemProjection>> SearchPublicLibraryAsync(
        Guid tenantId,
        Guid userId,
        bool includeWishlist,
        PublicLibraryListQuery query,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PublicFavoriteItemProjection>> SearchPublicFavoritesAsync(
        Guid tenantId,
        Guid userId,
        bool includeWishlist,
        string? sortBy,
        string? sortDirection,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<PublicCustomFieldValueProjection>>> GetPublicCustomFieldsByLibraryItemIdsAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> libraryItemIds,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PublicReviewProjection>> SearchPublicReviewsAsync(
        Guid tenantId,
        Guid userId,
        string? sortBy,
        string? sortDirection,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PublicActivityProjection>> SearchPublicActivityAsync(
        Guid tenantId,
        Guid userId,
        bool includeWishlist,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
