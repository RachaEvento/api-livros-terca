using FluentValidation;
using MeuAcervo.Application.DTOs.CustomFields;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Validators.CustomFields;

public sealed class CreateCustomFieldDefinitionRequestValidator : AbstractValidator<CreateCustomFieldDefinitionRequest>
{
    public CreateCustomFieldDefinitionRequestValidator()
    {
        RuleFor(request => request.Key)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Label)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(request => request.ConfigurationJson)
            .MaximumLength(4000);

        RuleForEach(request => request.Options)
            .SetValidator(new CustomFieldOptionRequestValidator());

        RuleFor(request => request)
            .Must(HaveOptionsForListType)
            .WithMessage("List custom fields must define at least one option.");

        RuleFor(request => request)
            .Must(NotHaveOptionsForNonListType)
            .WithMessage("Only list custom fields can define options.");
    }

    private static bool HaveOptionsForListType(CreateCustomFieldDefinitionRequest request)
    {
        return request.DataType != CustomFieldDataType.List || request.Options.Count > 0;
    }

    private static bool NotHaveOptionsForNonListType(CreateCustomFieldDefinitionRequest request)
    {
        return request.DataType == CustomFieldDataType.List || request.Options.Count == 0;
    }
}
