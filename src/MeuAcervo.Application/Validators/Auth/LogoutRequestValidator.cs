using FluentValidation;
using MeuAcervo.Application.DTOs.Auth;

namespace MeuAcervo.Application.Validators.Auth;

public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty()
            .MaximumLength(2048);
    }
}
