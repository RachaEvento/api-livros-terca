using FluentValidation;
using MeuAcervo.Application.DTOs.CustomFields;

namespace MeuAcervo.Application.Validators.CustomFields;

public sealed class UpsertLibraryItemCustomFieldValuesRequestValidator : AbstractValidator<UpsertLibraryItemCustomFieldValuesRequest>
{
    public UpsertLibraryItemCustomFieldValuesRequestValidator()
    {
        RuleForEach(request => request.Values)
            .SetValidator(new CustomFieldValueInputRequestValidator());

        RuleFor(request => request.Values)
            .Must(values => values.Select(value => value.FieldKey.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() == values.Count)
            .WithMessage("FieldKey values must not repeat in the same request.");
    }
}
