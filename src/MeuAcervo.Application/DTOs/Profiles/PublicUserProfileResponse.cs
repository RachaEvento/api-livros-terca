namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record PublicUserProfileResponse(
    string Username,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    string? FavoriteQuoteOrHeadline,
    bool IsWishlistPublic,
    bool IsStatsPublic,
    bool IsRecentActivityPublic,
    PublicProfileStatisticsResponse? Statistics);
