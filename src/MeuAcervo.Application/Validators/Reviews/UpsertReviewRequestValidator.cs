using FluentValidation;
using MeuAcervo.Application.DTOs.Reviews;

namespace MeuAcervo.Application.Validators.Reviews;

public sealed class UpsertReviewRequestValidator : AbstractValidator<UpsertReviewRequest>
{
    public UpsertReviewRequestValidator()
    {
        RuleFor(request => request.Rating)
            .InclusiveBetween(1, 5);

        RuleFor(request => request.Title)
            .MaximumLength(200);

        RuleFor(request => request.Content)
            .NotEmpty()
            .MaximumLength(8000);
    }
}
