using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Abstractions.Reviews;

public interface IReviewRepository
{
    Task<Review?> GetByLibraryItemAsync(Guid tenantId, Guid userId, Guid libraryItemId, bool tracking, CancellationToken cancellationToken = default);

    void Add(Review review);

    void Remove(Review review);
}
