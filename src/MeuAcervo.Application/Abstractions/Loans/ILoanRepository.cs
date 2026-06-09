using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Abstractions.Loans;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(Guid tenantId, Guid loanId, bool tracking, CancellationToken cancellationToken = default);

    Task<Loan?> GetActiveByLibraryItemAsync(Guid tenantId, Guid libraryItemId, CancellationToken cancellationToken = default);

    Task<PagedResult<Loan>> SearchAsync(Guid tenantId, Guid userId, string? search, LoanStatus? status, bool? onlyActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    void Add(Loan loan);
}
