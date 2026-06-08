using FluentValidation;
using MeuAcervo.Application.DTOs.Books;

namespace MeuAcervo.Application.Validators.Books;

public sealed class BookImportRequestValidator : AbstractValidator<BookImportRequest>
{
    public BookImportRequestValidator()
    {
        RuleFor(request => request.Source)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(request => request.WorkTitle)
            .MaximumLength(500);

        RuleFor(request => request.Subtitle)
            .MaximumLength(500);

        RuleFor(request => request.Publisher)
            .MaximumLength(200);

        RuleFor(request => request.Language)
            .MaximumLength(16);

        RuleFor(request => request.ExternalId)
            .MaximumLength(200);

        RuleFor(request => request.WorkExternalId)
            .MaximumLength(200);

        RuleFor(request => request.CoverImageUrl)
            .MaximumLength(2048);

        RuleFor(request => request.ExternalUrl)
            .MaximumLength(2048);

        RuleFor(request => request.Description)
            .MaximumLength(8000);

        RuleFor(request => request.PageCount)
            .GreaterThan(0)
            .When(request => request.PageCount.HasValue);

        RuleFor(request => request.PublishedYear)
            .InclusiveBetween(1400, DateTime.UtcNow.Year + 1)
            .When(request => request.PublishedYear.HasValue);

        RuleFor(request => request.FirstPublishedYear)
            .InclusiveBetween(1400, DateTime.UtcNow.Year + 1)
            .When(request => request.FirstPublishedYear.HasValue);

        RuleForEach(request => request.Authors)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request)
            .Must(HasImportIdentifiers)
            .WithMessage("At least one provider identifier or ISBN must be provided for import.");
    }

    private static bool HasImportIdentifiers(BookImportRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.ExternalId)
               || !string.IsNullOrWhiteSpace(request.Isbn10)
               || !string.IsNullOrWhiteSpace(request.Isbn13);
    }
}
