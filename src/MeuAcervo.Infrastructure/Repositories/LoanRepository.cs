using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.Loans;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;
using MeuAcervo.Infrastructure.Data;

namespace MeuAcervo.Infrastructure.Repositories;

public sealed class LoanRepository : ILoanRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LoanRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Loan?> GetByIdAsync(Guid tenantId, Guid loanId, bool tracking, CancellationToken cancellationToken = default)
    {
        var query = tracking
            ? BaseQuery()
            : BaseQuery().AsNoTracking();

        return query.FirstOrDefaultAsync(loan => loan.TenantId == tenantId && loan.Id == loanId, cancellationToken);
    }

    public Task<Loan?> GetActiveByLibraryItemAsync(Guid tenantId, Guid libraryItemId, CancellationToken cancellationToken = default)
    {
        return BaseQuery()
            .FirstOrDefaultAsync(
                loan => loan.TenantId == tenantId
                        && loan.UserLibraryItemId == libraryItemId
                        && loan.Status == LoanStatus.Active
                        && loan.ReturnedAtUtc == null,
                cancellationToken);
    }

    public void Add(Loan loan)
    {
        _dbContext.Loans.Add(loan);
    }

    private IQueryable<Loan> BaseQuery()
    {
        return _dbContext.Loans
            .Include(loan => loan.UserLibraryItem)
                .ThenInclude(item => item!.BookEdition)
                    .ThenInclude(edition => edition!.BookWork)
            .AsSplitQuery();
    }
}
