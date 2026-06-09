namespace MeuAcervo.Application.DTOs.Loans;

public sealed record CreateLoanRequest(
    string BorrowerName,
    string? BorrowerContact,
    DateTime? LoanedAtUtc,
    DateTime? DueAtUtc,
    string? Notes);
