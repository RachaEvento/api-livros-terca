namespace MeuAcervo.Application.DTOs.Loans;

public sealed record ReturnLoanRequest(
    DateTime? ReturnedAtUtc);
