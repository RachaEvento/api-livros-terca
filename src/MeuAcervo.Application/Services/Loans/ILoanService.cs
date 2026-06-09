using MeuAcervo.Application.DTOs.Loans;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Services.Loans;

public interface ILoanService
{
    Task<PagedResult<LoanResponse>> GetLoansAsync(Guid tenantId, Guid userId, GetLoansRequest request, CancellationToken cancellationToken = default);

    Task<LoanResponse> GetByIdAsync(Guid tenantId, Guid userId, Guid loanId, CancellationToken cancellationToken = default);

    Task<LoanResponse> CreateAsync(Guid tenantId, Guid userId, Guid libraryItemId, CreateLoanRequest request, CancellationToken cancellationToken = default);

    Task<LoanResponse> ReturnAsync(Guid tenantId, Guid userId, Guid loanId, ReturnLoanRequest request, CancellationToken cancellationToken = default);
}
