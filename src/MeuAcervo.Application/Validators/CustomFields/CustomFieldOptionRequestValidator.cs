using FluentValidation;
using MeuAcervo.Application.DTOs.CustomFields;

namespace MeuAcervo.Application.Validators.CustomFields;

public sealed class CustomFieldOptionRequestValidator : AbstractValidator<CustomFieldOptionRequest>
{
    public CustomFieldOptionRequestValidator()
    {
        RuleFor(request => request.Value)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Label)
            .NotEmpty()
            .MaximumLength(150);
    }
}
