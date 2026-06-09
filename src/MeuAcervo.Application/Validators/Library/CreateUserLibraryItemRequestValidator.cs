using FluentValidation;
using MeuAcervo.Application.DTOs.Books;
using MeuAcervo.Application.DTOs.Library;
using MeuAcervo.Application.Validators.Books;

namespace MeuAcervo.Application.Validators.Library;

public sealed class CreateUserLibraryItemRequestValidator : AbstractValidator<CreateUserLibraryItemRequest>
{
    public CreateUserLibraryItemRequestValidator()
    {
        RuleFor(request => request)
            .Must(HasSingleBookSource)
            .WithMessage("Provide either BookEditionId or Book when creating a library item.");

        RuleFor(request => request.BookEditionId)
            .NotEmpty()
            .When(request => request.BookEditionId.HasValue);

        RuleFor(request => request.Book!)
            .SetValidator(new BookImportRequestValidator())
            .When(request => request.Book is not null);

        RuleFor(request => request.CurrentPage)
            .GreaterThan(0)
            .When(request => request.CurrentPage.HasValue);

        RuleFor(request => request.ProgressPercent)
            .InclusiveBetween(0m, 100m)
            .When(request => request.ProgressPercent.HasValue);

        RuleFor(request => request.ReadCount)
            .GreaterThanOrEqualTo(0)
            .When(request => request.ReadCount.HasValue);

        RuleFor(request => request.PhysicalLocation)
            .MaximumLength(256);

        RuleFor(request => request.Condition)
            .MaximumLength(100);

        RuleFor(request => request.PrivateNotes)
            .MaximumLength(4000);
    }

    private static bool HasSingleBookSource(CreateUserLibraryItemRequest request)
    {
        var hasEditionId = request.BookEditionId.HasValue && request.BookEditionId.Value != Guid.Empty;
        var hasBookPayload = request.Book is not null;

        return hasEditionId ^ hasBookPayload;
    }
}
