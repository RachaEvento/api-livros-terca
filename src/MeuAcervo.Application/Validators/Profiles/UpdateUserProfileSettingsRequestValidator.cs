using FluentValidation;
using MeuAcervo.Application.DTOs.Profiles;

namespace MeuAcervo.Application.Validators.Profiles;

public sealed class UpdateUserProfileSettingsRequestValidator : AbstractValidator<UpdateUserProfileSettingsRequest>
{
    public UpdateUserProfileSettingsRequestValidator()
    {
        RuleFor(request => request.FavoriteQuoteOrHeadline)
            .MaximumLength(280);
    }
}
