using FluentValidation;
using MeuAcervo.Application.DTOs.Tags;

namespace MeuAcervo.Application.Validators.Tags;

public sealed class UpdateTagRequestValidator : AbstractValidator<UpdateTagRequest>
{
    public UpdateTagRequestValidator()
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
