using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.Reviews;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Infrastructure.Data;

namespace MeuAcervo.Infrastructure.Repositories;

public sealed class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReviewRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Review?> GetByLibraryItemAsync(Guid tenantId, Guid userId, Guid libraryItemId, bool tracking, CancellationToken cancellationToken = default)
    {
        var query = tracking
            ? _dbContext.Reviews.AsQueryable()
            : _dbContext.Reviews.AsNoTracking();

        return query.FirstOrDefaultAsync(
            review => review.TenantId == tenantId && review.UserId == userId && review.UserLibraryItemId == libraryItemId,
            cancellationToken);
    }

    public void Add(Review review)
    {
        _dbContext.Reviews.Add(review);
    }

    public void Remove(Review review)
    {
        _dbContext.Reviews.Remove(review);
    }
}
