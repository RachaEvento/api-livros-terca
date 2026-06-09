using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Library;

public sealed record UserLibraryItemLoanSummaryResponse(
    Guid Id,
    string BorrowerName,
    DateTime LoanedAtUtc,
    DateTime? DueAtUtc,
    DateTime? ReturnedAtUtc,
    LoanStatus Status);
