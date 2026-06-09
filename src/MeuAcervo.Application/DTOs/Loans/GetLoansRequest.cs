using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.Loans;

public sealed record GetLoansRequest(
    string? Search,
    LoanStatus? Status,
    bool? OnlyActive,
    int PageNumber = 1,
    int PageSize = 20);
