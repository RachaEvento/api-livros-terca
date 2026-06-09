using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.Loans;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;
using MeuAcervo.Infrastructure.Data;
using MeuAcervo.Shared.Pagination;

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

    public async Task<PagedResult<Loan>> SearchAsync(Guid tenantId, Guid userId, string? search, LoanStatus? status, bool? onlyActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = BaseQuery()
            .AsNoTracking()
            .Where(loan => loan.TenantId == tenantId && loan.UserLibraryItem!.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchPattern = $"%{search.Trim()}%";
            query = query.Where(loan =>
                EF.Functions.ILike(loan.BorrowerName, searchPattern) ||
                EF.Functions.ILike(loan.UserLibraryItem!.BookEdition!.Title, searchPattern) ||
                EF.Functions.ILike(loan.UserLibraryItem!.BookEdition!.BookWork!.CanonicalTitle, searchPattern));
        }

        if (onlyActive == true)
        {
            query = query.Where(loan => loan.Status == LoanStatus.Active && loan.ReturnedAtUtc == null);
        }
        else if (status.HasValue)
        {
            if (status == LoanStatus.Overdue)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(loan => loan.Status == LoanStatus.Active && loan.ReturnedAtUtc == null && loan.DueAtUtc < utcNow);
            }
            else
            {
                query = query.Where(loan => loan.Status == status.Value);
            }
        }

        query = query.OrderByDescending(loan => loan.LoanedAtUtc).ThenByDescending(loan => loan.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return new PagedResult<Loan>(items, pageNumber, pageSize, totalCount);
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
