using FluentValidation;
using MeuAcervo.Application.DTOs.Profiles;
using MeuAcervo.Application.Validators;

namespace MeuAcervo.Application.Validators.Profiles;

public sealed class GetPublicFavoritesRequestValidator : AbstractValidator<GetPublicFavoritesRequest>
{
    private static readonly string[] AllowedSortFields = ["title", "updatedat", "finishedat"];

    public GetPublicFavoritesRequestValidator()
    {
        Include(new PagedRequestValidator());

        RuleFor(request => request.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value) || AllowedSortFields.Contains(value.Trim().ToLowerInvariant()))
            .WithMessage("sortBy must be one of: title, updatedAt, finishedAt.");
    }
}
