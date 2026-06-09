using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.Library;
using MeuAcervo.Application.Models.Library;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Infrastructure.Data;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Infrastructure.Repositories;

public sealed class UserLibraryRepository : IUserLibraryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserLibraryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> BookEditionExistsAsync(Guid bookEditionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.BookEditions.AnyAsync(edition => edition.Id == bookEditionId, cancellationToken);
    }

    public Task<int?> GetBookEditionPageCountAsync(Guid bookEditionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.BookEditions
            .Where(edition => edition.Id == bookEditionId)
            .Select(edition => edition.PageCount)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ActiveItemExistsAsync(Guid tenantId, Guid userId, Guid bookEditionId, Guid? excludingItemId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.UserLibraryItems
            .Where(item => item.TenantId == tenantId && item.UserId == userId && item.BookEditionId == bookEditionId && !item.IsDeleted);

        if (excludingItemId.HasValue)
        {
            query = query.Where(item => item.Id != excludingItemId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ExistingUserLibraryItemMatch>> GetActiveItemsByEditionIdsAsync(
        Guid tenantId,
        Guid userId,
        IReadOnlyCollection<Guid> bookEditionIds,
        CancellationToken cancellationToken = default)
    {
        if (bookEditionIds.Count == 0)
        {
            return Array.Empty<ExistingUserLibraryItemMatch>();
        }

        return await _dbContext.UserLibraryItems
            .AsNoTracking()
            .Where(item => item.TenantId == tenantId && item.UserId == userId && !item.IsDeleted && bookEditionIds.Contains(item.BookEditionId))
            .Select(item => new ExistingUserLibraryItemMatch(item.BookEditionId, item.Id, item.ShelfType))
            .ToArrayAsync(cancellationToken);
    }

    public Task<UserLibraryItem?> GetTrackedItemAsync(Guid tenantId, Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        return ReadItemGraph(tracking: true, includeProgressEntries: false, includeDetails: true)
            .FirstOrDefaultAsync(item => item.TenantId == tenantId && item.UserId == userId && item.Id == itemId, cancellationToken);
    }

    public Task<UserLibraryItem?> GetReadonlyItemAsync(Guid tenantId, Guid userId, Guid itemId, bool includeProgressEntries, CancellationToken cancellationToken = default)
    {
        return ReadItemGraph(tracking: false, includeProgressEntries, includeDetails: true)
            .FirstOrDefaultAsync(item => item.TenantId == tenantId && item.UserId == userId && item.Id == itemId, cancellationToken);
    }

    public async Task<PagedResult<UserLibraryItem>> SearchAsync(Guid tenantId, Guid userId, UserLibraryItemListQuery query, CancellationToken cancellationToken = default)
    {
        var itemIdsQuery = _dbContext.UserLibraryItems
            .Where(item => item.TenantId == tenantId && item.UserId == userId && !item.IsDeleted);

        itemIdsQuery = ApplyFilters(itemIdsQuery, query);
        itemIdsQuery = ApplySorting(itemIdsQuery, query);

        var totalCount = await itemIdsQuery.CountAsync(cancellationToken);
        var itemIds = await itemIdsQuery
            .Select(item => item.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);

        if (itemIds.Length == 0)
        {
            return new PagedResult<UserLibraryItem>([], query.PageNumber, query.PageSize, totalCount);
        }

        var items = await ReadItemGraph(tracking: false, includeProgressEntries: false, includeDetails: false)
            .Where(item => itemIds.Contains(item.Id))
            .ToListAsync(cancellationToken);

        var itemsById = items.ToDictionary(item => item.Id);
        var orderedItems = itemIds.Select(itemId => itemsById[itemId]).ToArray();

        return new PagedResult<UserLibraryItem>(orderedItems, query.PageNumber, query.PageSize, totalCount);
    }

    public void AddUserLibraryItem(UserLibraryItem item)
    {
        _dbContext.UserLibraryItems.Add(item);
    }

    public void AddReadingProgressEntry(ReadingProgressEntry entry)
    {
        _dbContext.ReadingProgressEntries.Add(entry);
    }

    public void RemoveUserLibraryItem(UserLibraryItem item)
    {
        _dbContext.UserLibraryItems.Remove(item);
    }

    private IQueryable<UserLibraryItem> ReadItemGraph(bool tracking, bool includeProgressEntries, bool includeDetails)
    {
        var query = tracking
            ? _dbContext.UserLibraryItems.AsQueryable()
            : _dbContext.UserLibraryItems.AsNoTracking();

        query = query
            .Include(item => item.BookEdition)
                .ThenInclude(edition => edition!.BookWork)
            .Include(item => item.BookEdition)
                .ThenInclude(edition => edition!.Publisher)
            .Include(item => item.BookEdition)
                .ThenInclude(edition => edition!.BookEditionAuthors)
                    .ThenInclude(link => link.Author)
            .AsSplitQuery();

        if (includeProgressEntries)
        {
            query = query.Include(item => item.ReadingProgressEntries);
        }

        if (includeDetails)
        {
            query = query
                .Include(item => item.Review)
                .Include(item => item.Loans);
        }

        return query;
    }

    private static IQueryable<UserLibraryItem> ApplyFilters(IQueryable<UserLibraryItem> query, UserLibraryItemListQuery filters)
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

        if (filters.UpdatedFrom.HasValue)
        {
            query = query.Where(item => item.UpdatedAtUtc >= filters.UpdatedFrom.Value);
        }

        if (filters.UpdatedTo.HasValue)
        {
            query = query.Where(item => item.UpdatedAtUtc <= filters.UpdatedTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Title))
        {
            var titlePattern = $"%{filters.Title.Trim()}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.BookEdition!.Title, titlePattern) ||
                EF.Functions.ILike(item.BookEdition!.BookWork!.CanonicalTitle, titlePattern));
        }

        if (!string.IsNullOrWhiteSpace(filters.Author))
        {
            var authorPattern = $"%{filters.Author.Trim()}%";
            query = query.Where(item =>
                item.BookEdition!.BookEditionAuthors.Any(link => EF.Functions.ILike(link.Author!.Name, authorPattern)));
        }

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var searchPattern = $"%{filters.Search.Trim()}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.BookEdition!.Title, searchPattern) ||
                EF.Functions.ILike(item.BookEdition!.BookWork!.CanonicalTitle, searchPattern) ||
                EF.Functions.ILike(item.PrivateNotes ?? string.Empty, searchPattern) ||
                item.BookEdition!.BookEditionAuthors.Any(link => EF.Functions.ILike(link.Author!.Name, searchPattern)));
        }

        return query;
    }

    private static IQueryable<UserLibraryItem> ApplySorting(IQueryable<UserLibraryItem> query, UserLibraryItemListQuery sorting)
    {
        var descending = string.Equals(sorting.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var sortBy = sorting.SortBy.Trim();

        return sortBy.ToLowerInvariant() switch
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
}
