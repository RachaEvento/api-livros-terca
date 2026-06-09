using MeuAcervo.Application.DTOs.Loans;

namespace MeuAcervo.Application.Services.Loans;

public interface ILoanService
{
    Task<LoanResponse> CreateAsync(Guid tenantId, Guid userId, Guid libraryItemId, CreateLoanRequest request, CancellationToken cancellationToken = default);

    Task<LoanResponse> ReturnAsync(Guid tenantId, Guid userId, Guid libraryItemId, Guid loanId, ReturnLoanRequest request, CancellationToken cancellationToken = default);
}
