using FluentValidation;
using MeuAcervo.Application.DTOs.Auth;

namespace MeuAcervo.Application.Validators.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.EmailOrUsername)
            .NotEmpty()
            .MaximumLength(320);

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}
