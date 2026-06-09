namespace MeuAcervo.Application.Models.Profiles;

public sealed record PublicProfileStatisticsProjection(
    int CollectionItemCount,
    int CompletedItemCount,
    int ReadingItemCount,
    int FavoriteItemCount,
    int PublicReviewCount,
    decimal? AveragePublicRating,
    int? WishlistItemCount);
