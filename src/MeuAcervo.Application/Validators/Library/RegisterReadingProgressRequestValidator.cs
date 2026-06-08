using FluentValidation;
using MeuAcervo.Application.DTOs.Library;

namespace MeuAcervo.Application.Validators.Library;

public sealed class RegisterReadingProgressRequestValidator : AbstractValidator<RegisterReadingProgressRequest>
{
    public RegisterReadingProgressRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .When(request => request.PageNumber.HasValue);

        RuleFor(request => request.ProgressPercent)
            .InclusiveBetween(0m, 100m)
            .When(request => request.ProgressPercent.HasValue);

        RuleFor(request => request.Notes)
            .MaximumLength(2000);

        RuleFor(request => request)
            .Must(request => request.PageNumber.HasValue || request.ProgressPercent.HasValue)
            .WithMessage("At least PageNumber or ProgressPercent must be provided.");
    }
}
