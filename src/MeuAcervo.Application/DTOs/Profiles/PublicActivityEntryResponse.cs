namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record PublicActivityEntryResponse(
    string ActivityType,
    DateTime OccurredAtUtc,
    Guid? LibraryItemId,
    Guid? ReviewId,
    Guid? BookEditionId,
    Guid? BookWorkId,
    string? BookTitle,
    string? CanonicalTitle,
    string? CoverImageUrl,
    int? Rating,
    string? ReviewTitle);
