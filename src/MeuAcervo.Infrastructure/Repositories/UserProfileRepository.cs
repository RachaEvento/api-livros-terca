using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.Profiles;
using MeuAcervo.Application.Models.Profiles;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;
using MeuAcervo.Infrastructure.Data;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Infrastructure.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserProfileRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserProfile?> GetByUserIdAsync(Guid tenantId, Guid userId, bool tracking, CancellationToken cancellationToken = default)
    {
        var query = tracking
            ? _dbContext.UserProfiles.AsQueryable()
            : _dbContext.UserProfiles.AsNoTracking();

        return query.FirstOrDefaultAsync(profile => profile.TenantId == tenantId && profile.UserId == userId, cancellationToken);
    }

    public void Add(UserProfile profile)
    {
        _dbContext.UserProfiles.Add(profile);
    }

    public Task<PublicProfileLookup?> GetPublicProfileLookupAsync(string normalizedUsername, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .Where(user => user.NormalizedUsername == normalizedUsername)
            .Select(user => new PublicProfileLookup(
                user.TenantId,
                user.Id,
                user.Username,
                user.DisplayName,
                user.AvatarUrl,
                user.Bio,
                user.UserProfile != null ? user.UserProfile.FavoriteQuoteOrHeadline : null,
                user.UserProfile != null && user.UserProfile.IsPublicProfileEnabled,
                user.UserProfile != null && user.UserProfile.IsWishlistPublic,
                user.UserProfile != null && user.UserProfile.IsStatsPublic,
                user.UserProfile != null && user.UserProfile.IsRecentActivityPublic))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PublicProfileStatisticsProjection> GetPublicStatisticsAsync(Guid tenantId, Guid userId, bool includeWishlist, CancellationToken cancellationToken = default)
    {
        var itemsQuery = _dbContext.UserLibraryItems
            .AsNoTracking()
            .Where(item => item.TenantId == tenantId && item.UserId == userId);

        var reviewsQuery = _dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.TenantId == tenantId && review.UserId == userId && review.Visibility == ReviewVisibility.Public);

        var collectionItemCount = await itemsQuery.CountAsync(item => item.ShelfType != ShelfType.Wishlist, cancellationToken);
        var completedItemCount = await itemsQuery.CountAsync(item => item.ReadingStatus == ReadingStatus.Completed, cancellationToken);
        var readingItemCount = await itemsQuery.CountAsync(
            item => item.ReadingStatus == ReadingStatus.Reading || item.ReadingStatus == ReadingStatus.Rereading,
            cancellationToken);
        var favoriteItemCount = await itemsQuery.CountAsync(
            item => item.IsFavorite && (includeWishlist || item.ShelfType != ShelfType.Wishlist),
            cancellationToken);
        var publicReviewCount = await reviewsQuery.CountAsync(cancellationToken);
        var averagePublicRating = await reviewsQuery
            .Select(review => (decimal?)review.Rating)
            .AverageAsync(cancellationToken);

        var wishlistItemCount = includeWishlist
            ? await itemsQuery.CountAsync(item => item.ShelfType == ShelfType.Wishlist, cancellationToken)
            : (int?)null;

        return new PublicProfileStatisticsProjection(
            collectionItemCount,
            completedItemCount,
            readingItemCount,
            favoriteItemCount,
            publicReviewCount,
            averagePublicRating,
            wishlistItemCount);
    }

    public async Task<PagedResult<PublicLibraryItemProjection>> SearchPublicLibraryAsync(
        Guid tenantId,
        Guid userId,
        bool includeWishlist,
        PublicLibraryListQuery query,
        CancellationToken cancellationToken = default)
    {
        var itemsQuery = BuildVisibleLibraryItemsQuery(tenantId, userId, includeWishlist);
        itemsQuery = ApplyPublicLibraryFilters(itemsQuery, query);
        itemsQuery = ApplyPublicLibrarySorting(itemsQuery, query.SortBy, query.SortDirection);

        var totalCount = await itemsQuery.CountAsync(cancellationToken);
        var itemIds = await itemsQuery
            .Select(item => item.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        if (itemIds.Length == 0)
        {
            return new PagedResult<PublicLibraryItemProjection>([], query.PageNumber, query.PageSize, totalCount);
        }

        var items = await _dbContext.UserLibraryItems
            .AsNoTracking()
            .Where(item => itemIds.Contains(item.Id))
            .Select(item => new PublicLibraryItemProjection(
                item.Id,
                item.BookEditionId,
                item.BookEdition!.BookWorkId,
                item.BookEdition.BookWork!.CanonicalTitle,
                item.BookEdition.Title,
                item.BookEdition.Subtitle,
                item.BookEdition.BookEditionAuthors
                    .OrderBy(link => link.ContributionOrder)
                    .Select(link => link.Author!.Name)
                    .ToArray(),
                item.BookEdition.Publisher != null ? item.BookEdition.Publisher.Name : null,
                item.BookEdition.Language,
                item.BookEdition.CoverImageUrl,
                item.ShelfType,
                item.ReadingStatus,
                item.AcquisitionFormat,
                item.IsFavorite,
                item.ProgressPercent,
                item.ReadCount,
                item.StartedAt,
                item.FinishedAt,
                item.AcquiredAt,
                item.CreatedAtUtc,
                item.UpdatedAtUtc,
                item.Review != null && item.Review.Visibility == ReviewVisibility.Public
                    ? new PublicLibraryItemReviewSummaryProjection(
                        item.Review.Id,
                        item.Review.Rating,
                        item.Review.Title,
                        item.Review.ContainsSpoilers,
                        item.Review.PublishedAtUtc)
                    : null))
            .ToListAsync(cancellationToken);

        var itemsById = items.ToDictionary(item => item.LibraryItemId);
        var orderedItems = itemIds.Select(itemId => itemsById[itemId]).ToArray();

        return new PagedResult<PublicLibraryItemProjection>(orderedItems, query.PageNumber, query.PageSize, totalCount);
    }

    public async Task<PagedResult<PublicFavoriteItemProjection>> SearchPublicFavoritesAsync(
        Guid tenantId,
        Guid userId,
        bool includeWishlist,
        string? sortBy,
        string? sortDirection,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.UserLibraryItems
            .AsNoTracking()
            .Where(item =>
                item.TenantId == tenantId &&
                item.UserId == userId &&
                item.IsFavorite &&
                (includeWishlist || item.ShelfType != ShelfType.Wishlist));

        query = ApplyFavoriteSorting(query, sortBy, sortDirection);

        var totalCount = await query.CountAsync(cancellationToken);
        var itemIds = await query
            .Select(item => item.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        if (itemIds.Length == 0)
        {
            return new PagedResult<PublicFavoriteItemProjection>([], pageNumber, pageSize, totalCount);
        }

        var items = await _dbContext.UserLibraryItems
            .AsNoTracking()
            .Where(item => itemIds.Contains(item.Id))
            .Select(item => new PublicFavoriteItemProjection(
                item.Id,
                item.BookEditionId,
                item.BookEdition!.BookWorkId,
                item.ShelfType,
                item.ReadingStatus,
                item.ProgressPercent,
                item.ReadCount,
                item.StartedAt,
                item.FinishedAt,
                item.UpdatedAtUtc,
                item.BookEdition!.Title,
                item.BookEdition.Subtitle,
                item.BookEdition.BookWork!.CanonicalTitle,
                item.BookEdition.Language,
                item.BookEdition.CoverImageUrl,
                item.BookEdition.Isbn13,
                item.BookEdition.PublishedAt,
                item.BookEdition.Publisher != null ? item.BookEdition.Publisher.Name : null,
                item.BookEdition.BookEditionAuthors
                    .OrderBy(link => link.ContributionOrder)
                    .Select(link => link.Author!.Name)
                    .ToArray()))
            .ToListAsync(cancellationToken);

        var itemsById = items.ToDictionary(item => item.LibraryItemId);
        var orderedItems = itemIds.Select(itemId => itemsById[itemId]).ToArray();

        return new PagedResult<PublicFavoriteItemProjection>(orderedItems, pageNumber, pageSize, totalCount);
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<PublicCustomFieldValueProjection>>> GetPublicCustomFieldsByLibraryItemIdsAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> libraryItemIds,
        CancellationToken cancellationToken = default)
    {
        if (libraryItemIds.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyCollection<PublicCustomFieldValueProjection>>();
        }

        var values = await _dbContext.CustomFieldValues
            .AsNoTracking()
            .Where(value =>
                value.TenantId == tenantId &&
                value.EntityType == CustomFieldEntityType.UserLibraryItem &&
                libraryItemIds.Contains(value.EntityId) &&
                value.CustomFieldDefinition != null &&
                value.CustomFieldDefinition.IsPublic)
            .OrderBy(value => value.CustomFieldDefinition!.SortOrder)
            .ThenBy(value => value.CustomFieldDefinition!.Label)
            .Select(value => new PublicCustomFieldValueProjection(
                value.EntityId,
                value.CustomFieldDefinitionId,
                value.CustomFieldDefinition!.Label,
                value.CustomFieldDefinition!.DataType,
                value.TextValue,
                value.NumberValue,
                value.DateValue,
                value.BooleanValue,
                value.OptionValue))
            .ToArrayAsync(cancellationToken);

        return values
            .GroupBy(value => value.LibraryItemId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyCollection<PublicCustomFieldValueProjection>)group.ToArray());
    }

    public async Task<PagedResult<PublicReviewProjection>> SearchPublicReviewsAsync(
        Guid tenantId,
        Guid userId,
        string? sortBy,
        string? sortDirection,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.TenantId == tenantId && review.UserId == userId && review.Visibility == ReviewVisibility.Public);

        query = ApplyReviewSorting(query, sortBy, sortDirection);

        var totalCount = await query.CountAsync(cancellationToken);
        var reviewIds = await query
            .Select(review => review.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        if (reviewIds.Length == 0)
        {
            return new PagedResult<PublicReviewProjection>([], pageNumber, pageSize, totalCount);
        }

        var reviews = await _dbContext.Reviews
            .AsNoTracking()
            .Where(review => reviewIds.Contains(review.Id))
            .Select(review => new PublicReviewProjection(
                review.Id,
                review.Rating,
                review.Title,
                review.Content,
                review.ContainsSpoilers,
                review.PublishedAtUtc,
                review.CreatedAtUtc,
                review.UpdatedAtUtc,
                review.UserLibraryItemId,
                review.UserLibraryItem!.BookEditionId,
                review.UserLibraryItem.BookEdition!.BookWorkId,
                review.UserLibraryItem.BookEdition!.Title,
                review.UserLibraryItem.BookEdition!.Subtitle,
                review.UserLibraryItem.BookEdition!.BookWork!.CanonicalTitle,
                review.UserLibraryItem.BookEdition!.CoverImageUrl,
                review.UserLibraryItem.BookEdition!.Publisher != null ? review.UserLibraryItem.BookEdition.Publisher.Name : null,
                review.UserLibraryItem.BookEdition!.BookEditionAuthors
                    .OrderBy(link => link.ContributionOrder)
                    .Select(link => link.Author!.Name)
                    .ToArray()))
            .ToListAsync(cancellationToken);

        var reviewsById = reviews.ToDictionary(review => review.ReviewId);
        var orderedReviews = reviewIds.Select(reviewId => reviewsById[reviewId]).ToArray();

        return new PagedResult<PublicReviewProjection>(orderedReviews, pageNumber, pageSize, totalCount);
    }

    public async Task<PagedResult<PublicActivityProjection>> SearchPublicActivityAsync(
        Guid tenantId,
        Guid userId,
        bool includeWishlist,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var visibleItems = _dbContext.UserLibraryItems
            .AsNoTracking()
            .Where(item =>
                item.TenantId == tenantId &&
                item.UserId == userId &&
                (includeWishlist || item.ShelfType != ShelfType.Wishlist));

        var publicReviews = _dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.TenantId == tenantId && review.UserId == userId && review.Visibility == ReviewVisibility.Public);

        var fetchLimit = pageNumber * pageSize;

        var startedCount = await visibleItems.CountAsync(item => item.StartedAt.HasValue, cancellationToken);
        var completedCount = await visibleItems.CountAsync(item => item.FinishedAt.HasValue, cancellationToken);
        var reviewCount = await publicReviews.CountAsync(cancellationToken);

        var startedItems = await visibleItems
            .Where(item => item.StartedAt.HasValue)
            .OrderByDescending(item => item.StartedAt)
            .Select(item => new PublicActivityProjection(
                "started",
                item.StartedAt!.Value,
                item.Id,
                null,
                item.BookEditionId,
                item.BookEdition!.BookWorkId,
                item.BookEdition!.Title,
                item.BookEdition!.BookWork!.CanonicalTitle,
                item.BookEdition!.CoverImageUrl,
                null,
                null))
            .Take(fetchLimit)
            .ToArrayAsync(cancellationToken);

        var completedItems = await visibleItems
            .Where(item => item.FinishedAt.HasValue)
            .OrderByDescending(item => item.FinishedAt)
            .Select(item => new PublicActivityProjection(
                "completed",
                item.FinishedAt!.Value,
                item.Id,
                null,
                item.BookEditionId,
                item.BookEdition!.BookWorkId,
                item.BookEdition!.Title,
                item.BookEdition!.BookWork!.CanonicalTitle,
                item.BookEdition!.CoverImageUrl,
                null,
                null))
            .Take(fetchLimit)
            .ToArrayAsync(cancellationToken);

        var reviewActivities = await publicReviews
            .OrderByDescending(review => review.PublishedAtUtc ?? review.UpdatedAtUtc)
            .Select(review => new PublicActivityProjection(
                "review-published",
                review.PublishedAtUtc ?? review.UpdatedAtUtc,
                review.UserLibraryItemId,
                review.Id,
                review.UserLibraryItem!.BookEditionId,
                review.UserLibraryItem.BookEdition!.BookWorkId,
                review.UserLibraryItem.BookEdition!.Title,
                review.UserLibraryItem.BookEdition!.BookWork!.CanonicalTitle,
                review.UserLibraryItem.BookEdition!.CoverImageUrl,
                review.Rating,
                review.Title))
            .Take(fetchLimit)
            .ToArrayAsync(cancellationToken);

        var merged = startedItems
            .Concat(completedItems)
            .Concat(reviewActivities)
            .OrderByDescending(activity => activity.OccurredAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        var totalCount = startedCount + completedCount + reviewCount;
        return new PagedResult<PublicActivityProjection>(merged, pageNumber, pageSize, totalCount);
    }

    private IQueryable<UserLibraryItem> BuildVisibleLibraryItemsQuery(Guid tenantId, Guid userId, bool includeWishlist)
    {
        var query = _dbContext.UserLibraryItems
            .AsNoTracking()
            .Where(item =>
                item.TenantId == tenantId &&
                item.UserId == userId &&
                item.ShelfType != ShelfType.Archived);

        if (!includeWishlist)
        {
            query = query.Where(item => item.ShelfType != ShelfType.Wishlist);
        }

        return query;
    }

    private static IQueryable<UserLibraryItem> ApplyPublicLibraryFilters(IQueryable<UserLibraryItem> query, PublicLibraryListQuery filters)
    {
        if (filters.ShelfType.HasValue)
        {
            query = query.Where(item => item.ShelfType == filters.ShelfType.Value);
        }

        if (filters.ReadingStatus.HasValue)
        {
            query = query.Where(item => item.ReadingStatus == filters.ReadingStatus.Value);
        }

        if (filters.IsFavorite.HasValue)
        {
            query = query.Where(item => item.IsFavorite == filters.IsFavorite.Value);
        }

        if (filters.AcquisitionFormat.HasValue)
        {
            query = query.Where(item => item.AcquisitionFormat == filters.AcquisitionFormat.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Title))
        {
            var titlePattern = $"%{filters.Title}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.BookEdition!.Title, titlePattern) ||
                EF.Functions.ILike(item.BookEdition!.BookWork!.CanonicalTitle, titlePattern));
        }

        if (!string.IsNullOrWhiteSpace(filters.Author))
        {
            var authorPattern = $"%{filters.Author}%";
            query = query.Where(item =>
                item.BookEdition!.BookEditionAuthors.Any(link => EF.Functions.ILike(link.Author!.Name, authorPattern)));
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var searchPattern = $"%{filters.Search}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.BookEdition!.Title, searchPattern) ||
                EF.Functions.ILike(item.BookEdition!.BookWork!.CanonicalTitle, searchPattern) ||
                item.BookEdition!.BookEditionAuthors.Any(link => EF.Functions.ILike(link.Author!.Name, searchPattern)));
        }

        return query;
    }

    private static IQueryable<UserLibraryItem> ApplyPublicLibrarySorting(IQueryable<UserLibraryItem> query, string sortBy, string sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var normalizedSortBy = sortBy.Trim().ToLowerInvariant();

        return normalizedSortBy switch
        {
            "title" => descending
                ? query.OrderByDescending(item => item.BookEdition!.BookWork!.CanonicalTitle).ThenByDescending(item => item.BookEdition!.Title)
                : query.OrderBy(item => item.BookEdition!.BookWork!.CanonicalTitle).ThenBy(item => item.BookEdition!.Title),

            "author" => descending
                ? query.OrderByDescending(item => item.BookEdition!.BookEditionAuthors
                    .OrderBy(link => link.ContributionOrder)
                    .Select(link => link.Author!.Name)
                    .FirstOrDefault())
                    .ThenByDescending(item => item.BookEdition!.BookWork!.CanonicalTitle)
                : query.OrderBy(item => item.BookEdition!.BookEditionAuthors
                    .OrderBy(link => link.ContributionOrder)
                    .Select(link => link.Author!.Name)
                    .FirstOrDefault())
                    .ThenBy(item => item.BookEdition!.BookWork!.CanonicalTitle),

            "createdat" => descending
                ? query.OrderByDescending(item => item.CreatedAtUtc)
                : query.OrderBy(item => item.CreatedAtUtc),

            "progress" => descending
                ? query.OrderByDescending(item => item.ProgressPercent ?? 0m).ThenByDescending(item => item.UpdatedAtUtc)
                : query.OrderBy(item => item.ProgressPercent ?? 0m).ThenBy(item => item.UpdatedAtUtc),

            "finishedat" => descending
                ? query.OrderByDescending(item => item.FinishedAt).ThenByDescending(item => item.UpdatedAtUtc)
                : query.OrderBy(item => item.FinishedAt).ThenBy(item => item.UpdatedAtUtc),

            _ => descending
                ? query.OrderByDescending(item => item.UpdatedAtUtc)
                : query.OrderBy(item => item.UpdatedAtUtc)
        };
    }

    private static IQueryable<UserLibraryItem> ApplyFavoriteSorting(IQueryable<UserLibraryItem> query, string? sortBy, string? sortDirection)
    {
        var descending = !string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);
        var normalizedSortBy = sortBy?.Trim().ToLowerInvariant();

        return normalizedSortBy switch
        {
            "title" => descending
                ? query.OrderByDescending(item => item.BookEdition!.BookWork!.CanonicalTitle).ThenByDescending(item => item.BookEdition!.Title)
                : query.OrderBy(item => item.BookEdition!.BookWork!.CanonicalTitle).ThenBy(item => item.BookEdition!.Title),

            "finishedat" => descending
                ? query.OrderByDescending(item => item.FinishedAt).ThenByDescending(item => item.UpdatedAtUtc)
                : query.OrderBy(item => item.FinishedAt).ThenBy(item => item.UpdatedAtUtc),

            _ => descending
                ? query.OrderByDescending(item => item.UpdatedAtUtc)
                : query.OrderBy(item => item.UpdatedAtUtc)
        };
    }

    private static IQueryable<Review> ApplyReviewSorting(IQueryable<Review> query, string? sortBy, string? sortDirection)
    {
        var descending = !string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);
        var normalizedSortBy = sortBy?.Trim().ToLowerInvariant();

        return normalizedSortBy switch
        {
            "rating" => descending
                ? query.OrderByDescending(review => review.Rating).ThenByDescending(review => review.PublishedAtUtc ?? review.UpdatedAtUtc)
                : query.OrderBy(review => review.Rating).ThenBy(review => review.PublishedAtUtc ?? review.UpdatedAtUtc),

            "updatedat" => descending
                ? query.OrderByDescending(review => review.UpdatedAtUtc)
                : query.OrderBy(review => review.UpdatedAtUtc),

            _ => descending
                ? query.OrderByDescending(review => review.PublishedAtUtc ?? review.UpdatedAtUtc)
                : query.OrderBy(review => review.PublishedAtUtc ?? review.UpdatedAtUtc)
        };
    }
}
