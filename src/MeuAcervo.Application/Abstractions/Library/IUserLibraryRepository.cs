using MeuAcervo.Application.Models.Library;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Abstractions.Library;

public interface IUserLibraryRepository
{
    Task<bool> BookEditionExistsAsync(Guid bookEditionId, CancellationToken cancellationToken = default);

    Task<int?> GetBookEditionPageCountAsync(Guid bookEditionId, CancellationToken cancellationToken = default);

    Task<bool> ActiveItemExistsAsync(Guid tenantId, Guid userId, Guid bookEditionId, Guid? excludingItemId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ExistingUserLibraryItemMatch>> GetActiveItemsByEditionIdsAsync(
        Guid tenantId,
        Guid userId,
        IReadOnlyCollection<Guid> bookEditionIds,
        CancellationToken cancellationToken = default);

    Task<UserLibraryItem?> GetTrackedItemAsync(Guid tenantId, Guid userId, Guid itemId, CancellationToken cancellationToken = default);

    Task<UserLibraryItem?> GetReadonlyItemAsync(Guid tenantId, Guid userId, Guid itemId, bool includeProgressEntries, CancellationToken cancellationToken = default);

    Task<PagedResult<UserLibraryItem>> SearchAsync(Guid tenantId, Guid userId, UserLibraryItemListQuery query, CancellationToken cancellationToken = default);

    void AddUserLibraryItem(UserLibraryItem item);

    void AddReadingProgressEntry(ReadingProgressEntry entry);

    void RemoveUserLibraryItem(UserLibraryItem item);
}
