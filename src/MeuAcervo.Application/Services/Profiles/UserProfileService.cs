using FluentValidation;
using MeuAcervo.Application.Abstractions.Auth;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Abstractions.Profiles;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Profiles;
using MeuAcervo.Application.Models.Profiles;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Services.Profiles;

public sealed class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IValidator<UpdateUserProfileSettingsRequest> _updateUserProfileSettingsRequestValidator;
    private readonly IValidator<GetPublicLibraryItemsRequest> _getPublicLibraryItemsRequestValidator;
    private readonly IValidator<GetPublicFavoritesRequest> _getPublicFavoritesRequestValidator;
    private readonly IValidator<GetPublicReviewsRequest> _getPublicReviewsRequestValidator;
    private readonly IValidator<GetPublicActivityRequest> _getPublicActivityRequestValidator;

    public UserProfileService(
        IUserProfileRepository userProfileRepository,
        IIdentityRepository identityRepository,
        IApplicationDbContext applicationDbContext,
        IValidator<UpdateUserProfileSettingsRequest> updateUserProfileSettingsRequestValidator,
        IValidator<GetPublicLibraryItemsRequest> getPublicLibraryItemsRequestValidator,
        IValidator<GetPublicFavoritesRequest> getPublicFavoritesRequestValidator,
        IValidator<GetPublicReviewsRequest> getPublicReviewsRequestValidator,
        IValidator<GetPublicActivityRequest> getPublicActivityRequestValidator)
    {
        _userProfileRepository = userProfileRepository;
        _identityRepository = identityRepository;
        _applicationDbContext = applicationDbContext;
        _updateUserProfileSettingsRequestValidator = updateUserProfileSettingsRequestValidator;
        _getPublicLibraryItemsRequestValidator = getPublicLibraryItemsRequestValidator;
        _getPublicFavoritesRequestValidator = getPublicFavoritesRequestValidator;
        _getPublicReviewsRequestValidator = getPublicReviewsRequestValidator;
        _getPublicActivityRequestValidator = getPublicActivityRequestValidator;
    }

    public async Task<UserProfileSettingsResponse> GetCurrentAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetAuthenticatedUserAsync(tenantId, userId, cancellationToken);
        var profile = await _userProfileRepository.GetByUserIdAsync(tenantId, userId, tracking: false, cancellationToken);

        return MapCurrentResponse(user, profile);
    }

    public async Task<UserProfileSettingsResponse> UpdateCurrentAsync(Guid tenantId, Guid userId, UpdateUserProfileSettingsRequest request, CancellationToken cancellationToken = default)
    {
        await _updateUserProfileSettingsRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await GetAuthenticatedUserAsync(tenantId, userId, cancellationToken);
        var profile = await _userProfileRepository.GetByUserIdAsync(tenantId, userId, tracking: true, cancellationToken);

        if (profile is null)
        {
            profile = new UserProfile
            {
                TenantId = tenantId,
                UserId = userId
            };

            _userProfileRepository.Add(profile);
        }

        profile.IsPublicProfileEnabled = request.IsPublicProfileEnabled;
        profile.IsWishlistPublic = request.IsWishlistPublic;
        profile.IsStatsPublic = request.IsStatsPublic;
        profile.IsRecentActivityPublic = request.IsRecentActivityPublic;
        profile.FavoriteQuoteOrHeadline = TrimOrNull(request.FavoriteQuoteOrHeadline);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return MapCurrentResponse(user, profile);
    }

    public async Task<PublicUserProfileResponse> GetPublicProfileAsync(string username, CancellationToken cancellationToken = default)
    {
        var profile = await GetVisiblePublicProfileAsync(username, requireRecentActivity: false, cancellationToken);
        var statistics = profile.IsStatsPublic
            ? MapStatisticsResponse(await _userProfileRepository.GetPublicStatisticsAsync(profile.TenantId, profile.UserId, profile.IsWishlistPublic, cancellationToken))
            : null;

        return new PublicUserProfileResponse(
            profile.Username,
            profile.DisplayName,
            profile.AvatarUrl,
            profile.Bio,
            profile.FavoriteQuoteOrHeadline,
            profile.IsWishlistPublic,
            profile.IsStatsPublic,
            profile.IsRecentActivityPublic,
            statistics);
    }

    public async Task<PagedResult<PublicLibraryItemResponse>> GetPublicLibraryAsync(string username, GetPublicLibraryItemsRequest request, CancellationToken cancellationToken = default)
    {
        await _getPublicLibraryItemsRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var profile = await GetVisiblePublicProfileAsync(username, requireRecentActivity: false, cancellationToken);
        EnsureRequestedShelfIsVisible(profile, request.ShelfType);

        var query = new PublicLibraryListQuery(
            request.Search?.Trim(),
            request.Title?.Trim(),
            request.Author?.Trim(),
            request.ShelfType,
            request.ReadingStatus,
            request.IsFavorite,
            request.AcquisitionFormat,
            request.SortBy?.Trim() ?? "updatedAt",
            request.SortDirection?.Trim() ?? "desc",
            request.PageNumber,
            request.PageSize);

        var pagedResult = await _userProfileRepository.SearchPublicLibraryAsync(
            profile.TenantId,
            profile.UserId,
            profile.IsWishlistPublic,
            query,
            cancellationToken);

        var itemIds = pagedResult.Items.Select(item => item.LibraryItemId).ToArray();
        var publicCustomFields = itemIds.Length == 0
            ? new Dictionary<Guid, IReadOnlyCollection<PublicCustomFieldValueProjection>>()
            : new Dictionary<Guid, IReadOnlyCollection<PublicCustomFieldValueProjection>>(await _userProfileRepository.GetPublicCustomFieldsByLibraryItemIdsAsync(profile.TenantId, itemIds, cancellationToken));

        var responseItems = pagedResult.Items
            .Select(item => new PublicLibraryItemResponse(
                item.LibraryItemId,
                item.BookEditionId,
                item.BookWorkId,
                item.WorkTitle,
                item.EditionTitle,
                item.Subtitle,
                item.Authors,
                item.Publisher,
                item.Language,
                item.CoverImageUrl,
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
                item.Review is null
                    ? null
                    : new PublicLibraryItemReviewSummaryResponse(
                        item.Review.ReviewId,
                        item.Review.Rating,
                        item.Review.Title,
                        item.Review.ContainsSpoilers,
                        item.Review.PublishedAtUtc),
                publicCustomFields.TryGetValue(item.LibraryItemId, out var customFields)
                    ? customFields.Select(MapPublicCustomFieldResponse).ToArray()
                    : []))
            .ToArray();

        return new PagedResult<PublicLibraryItemResponse>(responseItems, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
    }

    public async Task<PagedResult<PublicFavoriteItemResponse>> GetPublicFavoritesAsync(string username, GetPublicFavoritesRequest request, CancellationToken cancellationToken = default)
    {
        await _getPublicFavoritesRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var profile = await GetVisiblePublicProfileAsync(username, requireRecentActivity: false, cancellationToken);
        var pagedResult = await _userProfileRepository.SearchPublicFavoritesAsync(
            profile.TenantId,
            profile.UserId,
            profile.IsWishlistPublic,
            request.SortBy,
            request.SortDirection,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var itemIds = pagedResult.Items.Select(item => item.LibraryItemId).ToArray();
        var publicCustomFields = itemIds.Length == 0
            ? new Dictionary<Guid, IReadOnlyCollection<PublicCustomFieldValueProjection>>()
            : new Dictionary<Guid, IReadOnlyCollection<PublicCustomFieldValueProjection>>(await _userProfileRepository.GetPublicCustomFieldsByLibraryItemIdsAsync(profile.TenantId, itemIds, cancellationToken));

        var responseItems = pagedResult.Items
            .Select(item => new PublicFavoriteItemResponse(
                item.LibraryItemId,
                item.BookEditionId,
                item.BookWorkId,
                item.ShelfType,
                item.ReadingStatus,
                item.ProgressPercent,
                item.ReadCount,
                item.StartedAt,
                item.FinishedAt,
                item.UpdatedAtUtc,
                item.Title,
                item.Subtitle,
                item.CanonicalTitle,
                item.Language,
                item.CoverImageUrl,
                item.Isbn13,
                item.PublishedAt,
                item.PublisherName,
                item.Authors,
                publicCustomFields.TryGetValue(item.LibraryItemId, out var customFields)
                    ? customFields.Select(MapPublicCustomFieldResponse).ToArray()
                    : []))
            .ToArray();

        return new PagedResult<PublicFavoriteItemResponse>(responseItems, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
    }

    public async Task<PagedResult<PublicReviewResponse>> GetPublicReviewsAsync(string username, GetPublicReviewsRequest request, CancellationToken cancellationToken = default)
    {
        await _getPublicReviewsRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var profile = await GetVisiblePublicProfileAsync(username, requireRecentActivity: false, cancellationToken);
        var pagedResult = await _userProfileRepository.SearchPublicReviewsAsync(
            profile.TenantId,
            profile.UserId,
            request.SortBy,
            request.SortDirection,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var responseItems = pagedResult.Items
            .Select(review => new PublicReviewResponse(
                review.ReviewId,
                review.Rating,
                review.Title,
                review.Content,
                review.ContainsSpoilers,
                review.PublishedAtUtc,
                review.CreatedAtUtc,
                review.UpdatedAtUtc,
                review.LibraryItemId,
                review.BookEditionId,
                review.BookWorkId,
                review.BookTitle,
                review.BookSubtitle,
                review.CanonicalTitle,
                review.CoverImageUrl,
                review.PublisherName,
                review.Authors))
            .ToArray();

        return new PagedResult<PublicReviewResponse>(responseItems, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
    }

    public async Task<PagedResult<PublicActivityEntryResponse>> GetPublicActivityAsync(string username, GetPublicActivityRequest request, CancellationToken cancellationToken = default)
    {
        await _getPublicActivityRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var profile = await GetVisiblePublicProfileAsync(username, requireRecentActivity: true, cancellationToken);
        var pagedResult = await _userProfileRepository.SearchPublicActivityAsync(
            profile.TenantId,
            profile.UserId,
            profile.IsWishlistPublic,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var responseItems = pagedResult.Items
            .Select(activity => new PublicActivityEntryResponse(
                activity.ActivityType,
                activity.OccurredAtUtc,
                activity.LibraryItemId,
                activity.ReviewId,
                activity.BookEditionId,
                activity.BookWorkId,
                activity.BookTitle,
                activity.CanonicalTitle,
                activity.CoverImageUrl,
                activity.Rating,
                activity.ReviewTitle))
            .ToArray();

        return new PagedResult<PublicActivityEntryResponse>(responseItems, pagedResult.PageNumber, pagedResult.PageSize, pagedResult.TotalCount);
    }

    private static void EnsureRequestedShelfIsVisible(PublicProfileLookup profile, Domain.Enums.ShelfType? requestedShelfType)
    {
        if (requestedShelfType == Domain.Enums.ShelfType.Wishlist && !profile.IsWishlistPublic)
        {
            throw new NotFoundException("Public library was not found for the informed username.");
        }
    }

    private async Task<User> GetAuthenticatedUserAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken)
    {
        var user = await _identityRepository.GetUserByIdAsync(userId, cancellationToken)
                   ?? throw new NotFoundException("Authenticated user was not found.");

        if (user.TenantId != tenantId)
        {
            throw new ForbiddenException("Authenticated session is not valid for this tenant.");
        }

        return user;
    }

    private async Task<PublicProfileLookup> GetVisiblePublicProfileAsync(string username, bool requireRecentActivity, CancellationToken cancellationToken)
    {
        var normalizedUsername = NormalizeUsername(username);
        var profile = await _userProfileRepository.GetPublicProfileLookupAsync(normalizedUsername, cancellationToken);

        if (profile is null || !profile.IsPublicProfileEnabled)
        {
            throw new NotFoundException("Public profile was not found for the informed username.");
        }

        if (requireRecentActivity && !profile.IsRecentActivityPublic)
        {
            throw new NotFoundException("Public activity was not found for the informed username.");
        }

        return profile;
    }

    private static UserProfileSettingsResponse MapCurrentResponse(User user, UserProfile? profile)
    {
        return new UserProfileSettingsResponse(
            profile?.Id,
            user.Username,
            user.DisplayName,
            profile?.IsPublicProfileEnabled ?? false,
            profile?.IsWishlistPublic ?? false,
            profile?.IsStatsPublic ?? true,
            profile?.IsRecentActivityPublic ?? false,
            profile?.FavoriteQuoteOrHeadline,
            profile?.UpdatedAtUtc ?? user.UpdatedAtUtc);
    }

    private static PublicProfileStatisticsResponse MapStatisticsResponse(PublicProfileStatisticsProjection statistics)
    {
        return new PublicProfileStatisticsResponse(
            statistics.CollectionItemCount,
            statistics.CompletedItemCount,
            statistics.ReadingItemCount,
            statistics.FavoriteItemCount,
            statistics.PublicReviewCount,
            statistics.AveragePublicRating,
            statistics.WishlistItemCount);
    }

    private static PublicCustomFieldValueResponse MapPublicCustomFieldResponse(PublicCustomFieldValueProjection customField)
    {
        return new PublicCustomFieldValueResponse(
            customField.DefinitionId,
            customField.Label,
            customField.DataType,
            customField.TextValue,
            customField.NumberValue,
            customField.DateValue,
            customField.BooleanValue,
            customField.OptionValue);
    }

    private static string NormalizeUsername(string username)
    {
        return username.Trim().ToUpperInvariant();
    }

    private static string? TrimOrNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
