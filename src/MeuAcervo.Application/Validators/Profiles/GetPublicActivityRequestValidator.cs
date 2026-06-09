using FluentValidation;
using MeuAcervo.Application.DTOs.Profiles;
using MeuAcervo.Application.Validators;

namespace MeuAcervo.Application.Validators.Profiles;

public sealed class GetPublicActivityRequestValidator : AbstractValidator<GetPublicActivityRequest>
{
    private static readonly string[] AllowedSortFields = ["occurredat"];

    public GetPublicActivityRequestValidator()
    {
        Include(new PagedRequestValidator());

        RuleFor(request => request.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value) || AllowedSortFields.Contains(value.Trim().ToLowerInvariant()))
            .WithMessage("sortBy must be one of: occurredAt.");
    }
}
