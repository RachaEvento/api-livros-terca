using FluentValidation;
using MeuAcervo.Application.DTOs.CustomFields;

namespace MeuAcervo.Application.Validators.CustomFields;

public sealed class CustomFieldValueInputRequestValidator : AbstractValidator<CustomFieldValueInputRequest>
{
    public CustomFieldValueInputRequestValidator()
    {
        RuleFor(request => request.DefinitionId)
            .NotEmpty();

        RuleFor(request => request.TextValue)
            .MaximumLength(4000);

        RuleFor(request => request.OptionValue)
            .MaximumLength(100);

        RuleFor(request => request)
            .Must(HaveSingleValue)
            .WithMessage("Each custom field value payload must provide exactly one typed value.");
    }

    private static bool HaveSingleValue(CustomFieldValueInputRequest request)
    {
        var count = 0;
        count += request.TextValue is not null ? 1 : 0;
        count += request.NumberValue.HasValue ? 1 : 0;
        count += request.DateValue.HasValue ? 1 : 0;
        count += request.BooleanValue.HasValue ? 1 : 0;
        count += request.OptionValue is not null ? 1 : 0;
        return count == 1;
    }
}
