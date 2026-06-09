using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Loans;

public sealed record LoanResponse(
    Guid Id,
    Guid UserLibraryItemId,
    Guid BookEditionId,
    string WorkTitle,
    string EditionTitle,
    string BorrowerName,
    string? BorrowerContact,
    DateTime LoanedAtUtc,
    DateTime? DueAtUtc,
    DateTime? ReturnedAtUtc,
    LoanStatus Status,
    string? Notes,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
