using FluentValidation;
using MeuAcervo.Application.DTOs.Loans;

namespace MeuAcervo.Application.Validators.Loans;

public sealed class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
{
    public CreateLoanRequestValidator()
    {
        RuleFor(request => request.BorrowerName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.BorrowerContact)
            .MaximumLength(200);

        RuleFor(request => request.Notes)
            .MaximumLength(2000);

        RuleFor(request => request)
            .Must(request => !request.DueAtUtc.HasValue || !request.LoanedAtUtc.HasValue || request.DueAtUtc >= request.LoanedAtUtc)
            .WithMessage("DueAtUtc must be later than or equal to LoanedAtUtc.");
    }
}
