using FluentValidation;
using MeuAcervo.Shared.Pagination;

namespace MeuAcervo.Application.Validators;

public sealed class PagedRequestValidator : AbstractValidator<PagedRequest>
{
    private static readonly string[] AllowedDirections = ["asc", "desc"];

    public PagedRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(request => request.SortDirection)
            .Must(direction => string.IsNullOrWhiteSpace(direction) || AllowedDirections.Contains(direction.Trim().ToLowerInvariant()))
            .WithMessage("SortDirection must be 'asc' or 'desc'.");
    }
}
