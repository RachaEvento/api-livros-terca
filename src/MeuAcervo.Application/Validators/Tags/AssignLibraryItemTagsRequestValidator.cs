using FluentValidation;
using MeuAcervo.Application.DTOs.Tags;

namespace MeuAcervo.Application.Validators.Tags;

public sealed class AssignLibraryItemTagsRequestValidator : AbstractValidator<AssignLibraryItemTagsRequest>
{
    public AssignLibraryItemTagsRequestValidator()
    {
        RuleFor(request => request.TagIds)
            .NotEmpty();

        RuleFor(request => request.TagIds)
            .Must(tagIds => tagIds.Distinct().Count() == tagIds.Count)
            .WithMessage("TagIds must not contain duplicates.");
    }
}
