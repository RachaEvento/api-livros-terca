using FluentValidation;
using MeuAcervo.Application.DTOs.Auth;

namespace MeuAcervo.Application.Validators.Auth;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.RefreshToken)
            .NotEmpty()
            .MaximumLength(2048);
    }
}
