using MeuAcervo.Application.DTOs.Reviews;

namespace MeuAcervo.Application.Services.Reviews;

public interface IReviewService
{
    Task<ReviewResponse> GetByLibraryItemAsync(Guid tenantId, Guid userId, Guid libraryItemId, CancellationToken cancellationToken = default);

    Task<ReviewResponse> CreateAsync(Guid tenantId, Guid userId, Guid libraryItemId, UpsertReviewRequest request, CancellationToken cancellationToken = default);

    Task<ReviewResponse> UpdateAsync(Guid tenantId, Guid userId, Guid libraryItemId, UpsertReviewRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid tenantId, Guid userId, Guid libraryItemId, CancellationToken cancellationToken = default);
}
