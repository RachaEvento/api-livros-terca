using FluentValidation;
using MeuAcervo.Application.DTOs.Loans;

namespace MeuAcervo.Application.Validators.Loans;

public sealed class GetLoansRequestValidator : AbstractValidator<GetLoansRequest>
{
    public GetLoansRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(request => request.Search)
            .MaximumLength(200);
    }
}
