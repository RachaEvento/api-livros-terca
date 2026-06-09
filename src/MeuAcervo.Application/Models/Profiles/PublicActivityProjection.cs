namespace MeuAcervo.Application.Models.Profiles;

public sealed record PublicActivityProjection(
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
