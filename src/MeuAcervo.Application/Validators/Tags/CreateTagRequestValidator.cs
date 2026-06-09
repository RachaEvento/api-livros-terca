using FluentValidation;
using MeuAcervo.Application.DTOs.Tags;

namespace MeuAcervo.Application.Validators.Tags;

public sealed class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Color)
            .MaximumLength(32);

        RuleFor(request => request.Description)
            .MaximumLength(500);
    }
}
