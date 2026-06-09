using FluentValidation;
using MeuAcervo.Application.DTOs.Profiles;
using MeuAcervo.Application.Validators;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Validators.Profiles;

public sealed class GetPublicLibraryItemsRequestValidator : AbstractValidator<GetPublicLibraryItemsRequest>
{
    private static readonly string[] AllowedSortFields = ["title", "author", "createdat", "updatedat", "progress", "finishedat"];

    public GetPublicLibraryItemsRequestValidator()
    {
        Include(new PagedRequestValidator());

        RuleFor(request => request.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value) || AllowedSortFields.Contains(value.Trim().ToLowerInvariant()))
            .WithMessage("sortBy must be one of: title, author, createdAt, updatedAt, progress, finishedAt.");

        RuleFor(request => request.Search)
            .MaximumLength(250);

        RuleFor(request => request.Title)
            .MaximumLength(250);

        RuleFor(request => request.Author)
            .MaximumLength(200);

        RuleFor(request => request.ShelfType)
            .Must(value => value is null || value == ShelfType.Collection || value == ShelfType.Wishlist)
            .WithMessage("shelfType must be Collection or Wishlist in the public library.");
    }
}
