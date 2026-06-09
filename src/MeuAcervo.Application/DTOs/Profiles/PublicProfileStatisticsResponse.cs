namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record PublicProfileStatisticsResponse(
    int CollectionItemCount,
    int CompletedItemCount,
    int ReadingItemCount,
    int FavoriteItemCount,
    int PublicReviewCount,
    decimal? AveragePublicRating,
    int? WishlistItemCount);
