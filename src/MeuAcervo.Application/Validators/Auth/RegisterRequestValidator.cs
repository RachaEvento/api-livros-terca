using System.Text.RegularExpressions;
using FluentValidation;
using MeuAcervo.Application.DTOs.Auth;

namespace MeuAcervo.Application.Validators.Auth;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$";

    public RegisterRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(request => request.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64)
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Username must contain only letters, numbers, dot, underscore or hyphen.");

        RuleFor(request => request.DisplayName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(request => request.Password)
            .NotEmpty()
            .Must(password => Regex.IsMatch(password, PasswordPattern))
            .WithMessage("Password must have at least 8 characters, including uppercase, lowercase, number and special character.");
    }
}
