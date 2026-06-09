using FluentValidation;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Abstractions.Library;
using MeuAcervo.Application.Abstractions.Loans;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Loans;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Services.Loans;

public sealed class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUserLibraryRepository _userLibraryRepository;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IValidator<CreateLoanRequest> _createLoanRequestValidator;
    private readonly IValidator<ReturnLoanRequest> _returnLoanRequestValidator;

    public LoanService(
        ILoanRepository loanRepository,
        IUserLibraryRepository userLibraryRepository,
        IApplicationDbContext applicationDbContext,
        IValidator<CreateLoanRequest> createLoanRequestValidator,
        IValidator<ReturnLoanRequest> returnLoanRequestValidator)
    {
        _loanRepository = loanRepository;
        _userLibraryRepository = userLibraryRepository;
        _applicationDbContext = applicationDbContext;
        _createLoanRequestValidator = createLoanRequestValidator;
        _returnLoanRequestValidator = returnLoanRequestValidator;
    }

    public async Task<LoanResponse> CreateAsync(Guid tenantId, Guid userId, Guid libraryItemId, CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        await _createLoanRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var item = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, libraryItemId, cancellationToken)
                   ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        if (item.ShelfType == ShelfType.Wishlist)
        {
            throw new BusinessRuleException("Wishlist items are not eligible for loans.");
        }

        if (item.AcquisitionFormat != Domain.Enums.AcquisitionFormat.Physical)
        {
            throw new BusinessRuleException("Only physical library items are eligible for loans.");
        }

        var activeLoan = await _loanRepository.GetActiveByLibraryItemAsync(tenantId, libraryItemId, cancellationToken);
        if (activeLoan is not null)
        {
            throw new ConflictException("The informed library item already has an active loan.");
        }

        var loanedAtUtc = request.LoanedAtUtc?.ToUniversalTime() ?? DateTime.UtcNow;
        var dueAtUtc = request.DueAtUtc?.ToUniversalTime();
        if (dueAtUtc.HasValue && dueAtUtc.Value < loanedAtUtc)
        {
            throw new BusinessRuleException("DueAtUtc must be later than or equal to LoanedAtUtc.");
        }

        var loan = new Loan
        {
            TenantId = tenantId,
            UserLibraryItemId = libraryItemId,
            UserLibraryItem = item,
            BorrowerName = request.BorrowerName.Trim(),
            BorrowerContact = TrimOrNull(request.BorrowerContact),
            LoanedAtUtc = loanedAtUtc,
            DueAtUtc = dueAtUtc,
            Notes = TrimOrNull(request.Notes),
            Status = LoanStatus.Active
        };

        _loanRepository.Add(loan);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return await GetResponseAsync(tenantId, userId, libraryItemId, loan.Id, cancellationToken);
    }

    public async Task<LoanResponse> ReturnAsync(Guid tenantId, Guid userId, Guid libraryItemId, Guid loanId, ReturnLoanRequest request, CancellationToken cancellationToken = default)
    {
        await _returnLoanRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var loan = await GetOwnedLoanAsync(tenantId, userId, libraryItemId, loanId, tracking: true, cancellationToken);
        if (loan.ReturnedAtUtc.HasValue || loan.Status == LoanStatus.Returned)
        {
            throw new BusinessRuleException("The informed loan has already been returned.");
        }

        var returnedAtUtc = request.ReturnedAtUtc?.ToUniversalTime() ?? DateTime.UtcNow;
        if (returnedAtUtc < loan.LoanedAtUtc)
        {
            throw new BusinessRuleException("ReturnedAtUtc cannot be earlier than LoanedAtUtc.");
        }

        loan.ReturnedAtUtc = returnedAtUtc;
        loan.Status = LoanStatus.Returned;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return await GetResponseAsync(tenantId, userId, libraryItemId, loan.Id, cancellationToken);
    }

    private async Task<LoanResponse> GetResponseAsync(
        Guid tenantId,
        Guid userId,
        Guid libraryItemId,
        Guid loanId,
        CancellationToken cancellationToken)
    {
        var loan = await GetOwnedLoanAsync(tenantId, userId, libraryItemId, loanId, tracking: false, cancellationToken);
        return MapResponse(loan);
    }

    private async Task<Loan> GetOwnedLoanAsync(
        Guid tenantId,
        Guid userId,
        Guid libraryItemId,
        Guid loanId,
        bool tracking,
        CancellationToken cancellationToken)
    {
        var loan = await _loanRepository.GetByIdAsync(tenantId, loanId, tracking, cancellationToken)
                   ?? throw new NotFoundException("Loan was not found for the authenticated tenant.");

        if (loan.UserLibraryItem?.UserId != userId || loan.UserLibraryItemId != libraryItemId)
        {
            throw new NotFoundException("Loan was not found for the authenticated user.");
        }

        return loan;
    }

    private static LoanResponse MapResponse(Loan loan)
    {
        var item = loan.UserLibraryItem!;
        var edition = item.BookEdition!;
        var work = edition.BookWork!;
        var effectiveStatus = loan.Status == LoanStatus.Active && loan.ReturnedAtUtc is null && loan.DueAtUtc.HasValue && loan.DueAtUtc.Value < DateTime.UtcNow
            ? LoanStatus.Overdue
            : loan.Status;

        return new LoanResponse(
            loan.Id,
            loan.UserLibraryItemId,
            item.BookEditionId,
            work.CanonicalTitle,
            edition.Title,
            loan.BorrowerName,
            loan.BorrowerContact,
            loan.LoanedAtUtc,
            loan.DueAtUtc,
            loan.ReturnedAtUtc,
            effectiveStatus,
            loan.Notes,
            loan.CreatedAtUtc,
            loan.UpdatedAtUtc);
    }

    private static string? TrimOrNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
