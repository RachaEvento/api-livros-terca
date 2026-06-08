using FluentValidation;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Abstractions.Library;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Library;
using MeuAcervo.Application.Models.Library;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Services.Library;

public sealed class UserLibraryService : IUserLibraryService
{
    private readonly IUserLibraryRepository _userLibraryRepository;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IValidator<GetUserLibraryItemsRequest> _getUserLibraryItemsRequestValidator;
    private readonly IValidator<CreateUserLibraryItemRequest> _createUserLibraryItemRequestValidator;
    private readonly IValidator<UpdateUserLibraryItemRequest> _updateUserLibraryItemRequestValidator;
    private readonly IValidator<UpdateUserLibraryItemStatusRequest> _updateUserLibraryItemStatusRequestValidator;
    private readonly IValidator<RegisterReadingProgressRequest> _registerReadingProgressRequestValidator;

    public UserLibraryService(
        IUserLibraryRepository userLibraryRepository,
        IApplicationDbContext applicationDbContext,
        IValidator<GetUserLibraryItemsRequest> getUserLibraryItemsRequestValidator,
        IValidator<CreateUserLibraryItemRequest> createUserLibraryItemRequestValidator,
        IValidator<UpdateUserLibraryItemRequest> updateUserLibraryItemRequestValidator,
        IValidator<UpdateUserLibraryItemStatusRequest> updateUserLibraryItemStatusRequestValidator,
        IValidator<RegisterReadingProgressRequest> registerReadingProgressRequestValidator)
    {
        _userLibraryRepository = userLibraryRepository;
        _applicationDbContext = applicationDbContext;
        _getUserLibraryItemsRequestValidator = getUserLibraryItemsRequestValidator;
        _createUserLibraryItemRequestValidator = createUserLibraryItemRequestValidator;
        _updateUserLibraryItemRequestValidator = updateUserLibraryItemRequestValidator;
        _updateUserLibraryItemStatusRequestValidator = updateUserLibraryItemStatusRequestValidator;
        _registerReadingProgressRequestValidator = registerReadingProgressRequestValidator;
    }

    public async Task<PagedResult<UserLibraryItemListResponse>> GetItemsAsync(Guid tenantId, Guid userId, GetUserLibraryItemsRequest request, CancellationToken cancellationToken = default)
    {
        await _getUserLibraryItemsRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var query = new UserLibraryItemListQuery(
            request.Search?.Trim(),
            request.Title?.Trim(),
            request.Author?.Trim(),
            request.ShelfType,
            request.ReadingStatus,
            request.IsFavorite,
            request.AcquisitionFormat,
            request.UpdatedFrom,
            request.UpdatedTo,
            request.SortBy?.Trim() ?? "updatedAt",
            request.SortDirection?.Trim() ?? "desc",
            request.PageNumber,
            request.PageSize);

        var pagedItems = await _userLibraryRepository.SearchAsync(tenantId, userId, query, cancellationToken);
        var responseItems = pagedItems.Items.Select(MapListResponse).ToArray();

        return new PagedResult<UserLibraryItemListResponse>(
            responseItems,
            pagedItems.PageNumber,
            pagedItems.PageSize,
            pagedItems.TotalCount);
    }

    public async Task<UserLibraryItemDetailResponse> CreateItemAsync(Guid tenantId, Guid userId, CreateUserLibraryItemRequest request, CancellationToken cancellationToken = default)
    {
        await _createUserLibraryItemRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await _userLibraryRepository.BookEditionExistsAsync(request.BookEditionId, cancellationToken))
        {
            throw new NotFoundException("The requested book edition was not found.");
        }

        if (await _userLibraryRepository.ActiveItemExistsAsync(tenantId, userId, request.BookEditionId, null, cancellationToken))
        {
            throw new ConflictException("The authenticated user already has an active library item for this edition.");
        }

        var pageCount = await _userLibraryRepository.GetBookEditionPageCountAsync(request.BookEditionId, cancellationToken);
        var item = new UserLibraryItem
        {
            TenantId = tenantId,
            UserId = userId,
            BookEditionId = request.BookEditionId,
            ShelfType = request.ShelfType,
            ReadingStatus = request.ReadingStatus ?? InferInitialReadingStatus(request.ShelfType, request.CurrentPage, request.ProgressPercent, pageCount),
            AcquisitionFormat = request.AcquisitionFormat,
            OwnershipType = request.OwnershipType,
            IsFavorite = request.IsFavorite,
            CurrentPage = request.CurrentPage,
            ProgressPercent = request.ProgressPercent,
            ReadCount = request.ReadCount ?? 0,
            StartedAt = request.StartedAt,
            FinishedAt = request.FinishedAt,
            AcquiredAt = request.AcquiredAt,
            PhysicalLocation = TrimOrNull(request.PhysicalLocation),
            Condition = TrimOrNull(request.Condition),
            PrivateNotes = TrimOrNull(request.PrivateNotes)
        };

        NormalizeItemState(item, previousStatus: null, requestedReadCount: request.ReadCount, pageCount, allowCompletionIncrement: false);

        _userLibraryRepository.AddUserLibraryItem(item);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return await GetItemByIdAsync(tenantId, userId, item.Id, cancellationToken);
    }

    public async Task<UserLibraryItemDetailResponse> GetItemByIdAsync(Guid tenantId, Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await _userLibraryRepository.GetReadonlyItemAsync(tenantId, userId, itemId, includeProgressEntries: true, cancellationToken)
                   ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        return MapDetailResponse(item);
    }

    public async Task<UserLibraryItemDetailResponse> UpdateItemAsync(Guid tenantId, Guid userId, Guid itemId, UpdateUserLibraryItemRequest request, CancellationToken cancellationToken = default)
    {
        await _updateUserLibraryItemRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var item = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, itemId, cancellationToken)
                   ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        var pageCount = item.BookEdition?.PageCount ?? await _userLibraryRepository.GetBookEditionPageCountAsync(item.BookEditionId, cancellationToken);
        var previousStatus = item.ReadingStatus;

        item.ShelfType = request.ShelfType;
        item.ReadingStatus = request.ReadingStatus;
        item.AcquisitionFormat = request.AcquisitionFormat;
        item.OwnershipType = request.OwnershipType;
        item.IsFavorite = request.IsFavorite;
        item.CurrentPage = request.CurrentPage;
        item.ProgressPercent = request.ProgressPercent;
        item.ReadCount = request.ReadCount;
        item.StartedAt = request.StartedAt;
        item.FinishedAt = request.FinishedAt;
        item.AcquiredAt = request.AcquiredAt;
        item.PhysicalLocation = TrimOrNull(request.PhysicalLocation);
        item.Condition = TrimOrNull(request.Condition);
        item.PrivateNotes = TrimOrNull(request.PrivateNotes);

        NormalizeItemState(item, previousStatus, request.ReadCount, pageCount, allowCompletionIncrement: true);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return await GetItemByIdAsync(tenantId, userId, item.Id, cancellationToken);
    }

    public async Task<UserLibraryItemDetailResponse> UpdateStatusAsync(Guid tenantId, Guid userId, Guid itemId, UpdateUserLibraryItemStatusRequest request, CancellationToken cancellationToken = default)
    {
        await _updateUserLibraryItemStatusRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var item = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, itemId, cancellationToken)
                   ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        var pageCount = item.BookEdition?.PageCount ?? await _userLibraryRepository.GetBookEditionPageCountAsync(item.BookEditionId, cancellationToken);
        var previousStatus = item.ReadingStatus;

        item.ReadingStatus = request.ReadingStatus;
        item.StartedAt = request.StartedAt ?? item.StartedAt;
        item.FinishedAt = request.FinishedAt;

        NormalizeItemState(item, previousStatus, requestedReadCount: null, pageCount, allowCompletionIncrement: true);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return await GetItemByIdAsync(tenantId, userId, item.Id, cancellationToken);
    }

    public async Task<UserLibraryItemDetailResponse> RegisterProgressAsync(Guid tenantId, Guid userId, Guid itemId, RegisterReadingProgressRequest request, CancellationToken cancellationToken = default)
    {
        await _registerReadingProgressRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var item = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, itemId, cancellationToken)
                   ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        if (item.ShelfType == ShelfType.Wishlist)
        {
            throw new BusinessRuleException("Wishlist items cannot receive reading progress entries.");
        }

        var pageCount = item.BookEdition?.PageCount ?? await _userLibraryRepository.GetBookEditionPageCountAsync(item.BookEditionId, cancellationToken);
        var pageNumber = request.PageNumber;
        if (pageCount.HasValue && pageNumber.HasValue && pageNumber.Value > pageCount.Value)
        {
            throw new BusinessRuleException("Reading progress page cannot exceed the edition page count.");
        }

        var normalizedProgressPercent = ResolveProgressPercent(pageNumber, request.ProgressPercent, pageCount);
        var recordedAtUtc = request.RecordedAtUtc?.ToUniversalTime() ?? DateTime.UtcNow;
        var previousStatus = item.ReadingStatus;

        var progressEntry = new ReadingProgressEntry
        {
            TenantId = tenantId,
            UserLibraryItemId = item.Id,
            PageNumber = pageNumber,
            ProgressPercent = normalizedProgressPercent,
            RecordedAtUtc = recordedAtUtc,
            Notes = TrimOrNull(request.Notes)
        };

        item.CurrentPage = pageNumber ?? item.CurrentPage;
        item.ProgressPercent = normalizedProgressPercent ?? item.ProgressPercent;

        ApplyProgressTransition(item, previousStatus, recordedAtUtc, pageCount);

        _userLibraryRepository.AddReadingProgressEntry(progressEntry);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return await GetItemByIdAsync(tenantId, userId, item.Id, cancellationToken);
    }

    public async Task DeleteItemAsync(Guid tenantId, Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, itemId, cancellationToken)
                   ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        _userLibraryRepository.RemoveUserLibraryItem(item);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    private static ReadingStatus InferInitialReadingStatus(ShelfType shelfType, int? currentPage, decimal? progressPercent, int? pageCount)
    {
        if (shelfType == ShelfType.Wishlist)
        {
            return ReadingStatus.NotStarted;
        }

        if (IsCompletionSnapshot(currentPage, progressPercent, pageCount))
        {
            return ReadingStatus.Completed;
        }

        return HasAnyProgress(currentPage, progressPercent)
            ? ReadingStatus.Reading
            : ReadingStatus.NotStarted;
    }

    private static void NormalizeItemState(
        UserLibraryItem item,
        ReadingStatus? previousStatus,
        int? requestedReadCount,
        int? pageCount,
        bool allowCompletionIncrement)
    {
        if (pageCount.HasValue && item.CurrentPage.HasValue && item.CurrentPage.Value > pageCount.Value)
        {
            throw new BusinessRuleException("CurrentPage cannot exceed the edition page count.");
        }

        if (item.ProgressPercent.HasValue && (item.ProgressPercent.Value < 0m || item.ProgressPercent.Value > 100m))
        {
            throw new BusinessRuleException("ProgressPercent must stay between 0 and 100.");
        }

        if (item.ShelfType == ShelfType.Wishlist && HasAnyProgress(item.CurrentPage, item.ProgressPercent))
        {
            throw new BusinessRuleException("Wishlist items cannot keep reading progress.");
        }

        if (pageCount.HasValue && item.CurrentPage.HasValue && item.ProgressPercent.HasValue)
        {
            var derivedProgress = decimal.Round((decimal)item.CurrentPage.Value / pageCount.Value * 100m, 2, MidpointRounding.AwayFromZero);
            if (Math.Abs(derivedProgress - item.ProgressPercent.Value) > 1m)
            {
                throw new BusinessRuleException("CurrentPage and ProgressPercent are inconsistent for the selected edition.");
            }
        }

        if (IsCompletionSnapshot(item.CurrentPage, item.ProgressPercent, pageCount))
        {
            item.ReadingStatus = ReadingStatus.Completed;
            item.ProgressPercent ??= 100m;

            if (pageCount.HasValue && !item.CurrentPage.HasValue)
            {
                item.CurrentPage = pageCount.Value;
            }
        }

        if (item.ReadingStatus == ReadingStatus.NotStarted && HasAnyProgress(item.CurrentPage, item.ProgressPercent))
        {
            throw new BusinessRuleException("A library item with progress cannot remain in NotStarted status.");
        }

        if (item.ReadingStatus == ReadingStatus.Completed)
        {
            item.FinishedAt ??= DateTime.UtcNow;
            item.ReadCount = requestedReadCount.HasValue
                ? Math.Max(requestedReadCount.Value, 1)
                : ComputeReadCountForCompletion(item.ReadCount, previousStatus, allowCompletionIncrement);
        }
        else
        {
            item.ReadCount = requestedReadCount ?? item.ReadCount;

            if (item.ReadingStatus is ReadingStatus.Reading or ReadingStatus.Rereading or ReadingStatus.Paused or ReadingStatus.Abandoned)
            {
                item.StartedAt ??= DateTime.UtcNow;
            }

            item.FinishedAt = null;
        }

        if (item.ReadCount < 0)
        {
            throw new BusinessRuleException("ReadCount cannot be negative.");
        }
    }

    private static void ApplyProgressTransition(UserLibraryItem item, ReadingStatus previousStatus, DateTime recordedAtUtc, int? pageCount)
    {
        if (IsCompletionSnapshot(item.CurrentPage, item.ProgressPercent, pageCount))
        {
            item.ReadingStatus = ReadingStatus.Completed;
            item.ProgressPercent = 100m;
            item.FinishedAt ??= recordedAtUtc;
            item.ReadCount = ComputeReadCountForCompletion(item.ReadCount, previousStatus, allowCompletionIncrement: true);
            return;
        }

        if (!HasAnyProgress(item.CurrentPage, item.ProgressPercent))
        {
            return;
        }

        if (previousStatus == ReadingStatus.Completed)
        {
            item.ReadingStatus = ReadingStatus.Rereading;
            item.FinishedAt = null;
        }
        else
        {
            item.ReadingStatus = ReadingStatus.Reading;
        }

        item.StartedAt ??= recordedAtUtc;
    }

    private static int ComputeReadCountForCompletion(int currentReadCount, ReadingStatus? previousStatus, bool allowCompletionIncrement)
    {
        if (!allowCompletionIncrement)
        {
            return Math.Max(currentReadCount, 1);
        }

        if (previousStatus == ReadingStatus.Completed)
        {
            return Math.Max(currentReadCount, 1);
        }

        return Math.Max(currentReadCount + 1, 1);
    }

    private static decimal? ResolveProgressPercent(int? pageNumber, decimal? progressPercent, int? pageCount)
    {
        if (progressPercent.HasValue && pageCount.HasValue && pageNumber.HasValue)
        {
            var derived = decimal.Round((decimal)pageNumber.Value / pageCount.Value * 100m, 2, MidpointRounding.AwayFromZero);
            if (Math.Abs(derived - progressPercent.Value) > 1m)
            {
                throw new BusinessRuleException("PageNumber and ProgressPercent are inconsistent for the selected edition.");
            }
        }

        if (progressPercent.HasValue)
        {
            return progressPercent.Value;
        }

        if (pageCount.HasValue && pageNumber.HasValue)
        {
            return decimal.Round((decimal)pageNumber.Value / pageCount.Value * 100m, 2, MidpointRounding.AwayFromZero);
        }

        return null;
    }

    private static bool IsCompletionSnapshot(int? currentPage, decimal? progressPercent, int? pageCount)
    {
        return progressPercent.HasValue && progressPercent.Value >= 100m
               || (pageCount.HasValue && currentPage.HasValue && currentPage.Value >= pageCount.Value);
    }

    private static bool HasAnyProgress(int? currentPage, decimal? progressPercent)
    {
        return currentPage.HasValue && currentPage.Value > 0
               || progressPercent.HasValue && progressPercent.Value > 0m;
    }

    private static string? TrimOrNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static UserLibraryItemListResponse MapListResponse(UserLibraryItem item)
    {
        var edition = item.BookEdition!;
        var work = edition.BookWork!;
        var authors = edition.BookEditionAuthors
            .OrderBy(link => link.ContributionOrder)
            .Select(link => link.Author?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .ToArray();

        return new UserLibraryItemListResponse(
            item.Id,
            item.BookEditionId,
            work.Id,
            work.CanonicalTitle,
            edition.Title,
            edition.Subtitle,
            authors,
            edition.Publisher?.Name,
            edition.Language,
            edition.CoverImageUrl,
            item.ShelfType,
            item.ReadingStatus,
            item.AcquisitionFormat,
            item.OwnershipType,
            item.IsFavorite,
            item.CurrentPage,
            item.ProgressPercent,
            item.ReadCount,
            item.StartedAt,
            item.FinishedAt,
            item.AcquiredAt,
            item.PhysicalLocation,
            item.Condition,
            item.PrivateNotes,
            item.CreatedAtUtc,
            item.UpdatedAtUtc);
    }

    private static UserLibraryItemDetailResponse MapDetailResponse(UserLibraryItem item)
    {
        var progressEntries = item.ReadingProgressEntries
            .OrderByDescending(entry => entry.RecordedAtUtc)
            .Select(entry => new ReadingProgressEntryResponse(
                entry.Id,
                entry.PageNumber,
                entry.ProgressPercent,
                entry.RecordedAtUtc,
                entry.Notes))
            .ToArray();

        return new UserLibraryItemDetailResponse(MapListResponse(item), progressEntries);
    }
}
