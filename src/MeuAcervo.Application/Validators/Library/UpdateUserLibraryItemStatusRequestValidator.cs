using FluentValidation;
using MeuAcervo.Application.DTOs.Library;

namespace MeuAcervo.Application.Validators.Library;

public sealed class UpdateUserLibraryItemStatusRequestValidator : AbstractValidator<UpdateUserLibraryItemStatusRequest>
{
    public UpdateUserLibraryItemStatusRequestValidator()
    {
        RuleFor(request => request)
            .Must(request => request.ReadingStatus != Domain.Enums.ReadingStatus.Completed || request.FinishedAt.HasValue)
            .WithMessage("FinishedAt must be provided when setting status to Completed.");
    }
}
