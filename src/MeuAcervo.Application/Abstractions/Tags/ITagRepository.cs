using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Abstractions.Tags;

public interface ITagRepository
{
    Task<IReadOnlyCollection<Tag>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default);

    Task<Tag?> GetByIdAsync(Guid tenantId, Guid tagId, bool tracking, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Tag>> GetByIdsAsync(Guid tenantId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(Guid tenantId, string slug, Guid? excludingTagId, CancellationToken cancellationToken = default);

    void Add(Tag tag);

    void Remove(Tag tag);

    void AddLibraryItemTag(UserLibraryItemTag link);

    void RemoveLibraryItemTag(UserLibraryItemTag link);
}
