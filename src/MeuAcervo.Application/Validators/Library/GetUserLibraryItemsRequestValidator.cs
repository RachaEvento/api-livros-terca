using FluentValidation;
using MeuAcervo.Application.DTOs.Library;

namespace MeuAcervo.Application.Validators.Library;

public sealed class GetUserLibraryItemsRequestValidator : AbstractValidator<GetUserLibraryItemsRequest>
{
    private static readonly string[] AllowedSortFields = ["title", "author", "createdAt", "updatedAt", "progress", "finishedAt"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public GetUserLibraryItemsRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(request => request.Search)
            .MaximumLength(500);

        RuleFor(request => request.Title)
            .MaximumLength(500);

        RuleFor(request => request.Author)
            .MaximumLength(200);

        RuleFor(request => request.SortBy)
            .Must(sortBy => string.IsNullOrWhiteSpace(sortBy) || AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortBy must be one of: title, author, createdAt, updatedAt, progress, finishedAt.");

        RuleFor(request => request.SortDirection)
            .Must(direction => string.IsNullOrWhiteSpace(direction) || AllowedSortDirections.Contains(direction, StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortDirection must be 'asc' or 'desc'.");

        RuleFor(request => request)
            .Must(request => !request.UpdatedFrom.HasValue || !request.UpdatedTo.HasValue || request.UpdatedFrom <= request.UpdatedTo)
            .WithMessage("UpdatedFrom must be earlier than or equal to UpdatedTo.");
    }
}
