using FluentValidation;
using MeuAcervo.Application.DTOs.Library;

namespace MeuAcervo.Application.Validators.Library;

public sealed class UpdateUserLibraryItemRequestValidator : AbstractValidator<UpdateUserLibraryItemRequest>
{
    public UpdateUserLibraryItemRequestValidator()
    {
        RuleFor(request => request.CurrentPage)
            .GreaterThan(0)
            .When(request => request.CurrentPage.HasValue);

        RuleFor(request => request.ProgressPercent)
            .InclusiveBetween(0m, 100m)
            .When(request => request.ProgressPercent.HasValue);

        RuleFor(request => request.ReadCount)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.PhysicalLocation)
            .MaximumLength(256);

        RuleFor(request => request.Condition)
            .MaximumLength(100);

        RuleFor(request => request.PrivateNotes)
            .MaximumLength(4000);
    }
}
