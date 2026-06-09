using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Models.Library;

public sealed record ExistingUserLibraryItemMatch(
    Guid BookEditionId,
    Guid LibraryItemId,
    ShelfType ShelfType);
