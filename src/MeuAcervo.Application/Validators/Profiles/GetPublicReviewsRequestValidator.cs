using FluentValidation;
using MeuAcervo.Application.DTOs.Profiles;
using MeuAcervo.Application.Validators;

namespace MeuAcervo.Application.Validators.Profiles;

public sealed class GetPublicReviewsRequestValidator : AbstractValidator<GetPublicReviewsRequest>
{
    private static readonly string[] AllowedSortFields = ["publishedat", "rating", "updatedat"];

    public GetPublicReviewsRequestValidator()
    {
        Include(new PagedRequestValidator());

        RuleFor(request => request.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value) || AllowedSortFields.Contains(value.Trim().ToLowerInvariant()))
            .WithMessage("sortBy must be one of: publishedAt, rating, updatedAt.");
    }
}
