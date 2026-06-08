using FluentValidation;
using MeuAcervo.Application.DTOs.Books;

namespace MeuAcervo.Application.Validators.Books;

public sealed class BookSearchRequestValidator : AbstractValidator<BookSearchRequest>
{
    public BookSearchRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(request => request)
            .Must(HasAtLeastOneSearchCriterion)
            .WithMessage("At least one search criterion must be provided.");

        RuleFor(request => request.Isbn)
            .MaximumLength(32);

        RuleFor(request => request.Title)
            .MaximumLength(500);

        RuleFor(request => request.Author)
            .MaximumLength(200);

        RuleFor(request => request.Search)
            .MaximumLength(500);

        RuleFor(request => request.Language)
            .MaximumLength(16);
    }

    private static bool HasAtLeastOneSearchCriterion(BookSearchRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.Search)
               || !string.IsNullOrWhiteSpace(request.Isbn)
               || !string.IsNullOrWhiteSpace(request.Title)
               || !string.IsNullOrWhiteSpace(request.Author);
    }
}
