using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.Tags;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Infrastructure.Data;

namespace MeuAcervo.Infrastructure.Repositories;

public sealed class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TagRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Tag>> GetAllAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tags
            .Where(tag => tag.TenantId == tenantId)
            .OrderBy(tag => tag.Name)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Tag?> GetByIdAsync(Guid tenantId, Guid tagId, bool tracking, CancellationToken cancellationToken = default)
    {
        var query = tracking
            ? _dbContext.Tags.AsQueryable()
            : _dbContext.Tags.AsNoTracking();

        return query.FirstOrDefaultAsync(tag => tag.TenantId == tenantId && tag.Id == tagId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Tag>> GetByIdsAsync(Guid tenantId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tags
            .Where(tag => tag.TenantId == tenantId && tagIds.Contains(tag.Id))
            .ToArrayAsync(cancellationToken);
    }

    public Task<bool> SlugExistsAsync(Guid tenantId, string slug, Guid? excludingTagId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Tags.Where(tag => tag.TenantId == tenantId && tag.Slug == slug);
        if (excludingTagId.HasValue)
        {
            query = query.Where(tag => tag.Id != excludingTagId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public void Add(Tag tag)
    {
        _dbContext.Tags.Add(tag);
    }

    public void Remove(Tag tag)
    {
        _dbContext.Tags.Remove(tag);
    }

    public void AddLibraryItemTag(UserLibraryItemTag link)
    {
        _dbContext.UserLibraryItemTags.Add(link);
    }

    public void RemoveLibraryItemTag(UserLibraryItemTag link)
    {
        _dbContext.UserLibraryItemTags.Remove(link);
    }
}
