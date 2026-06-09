namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record UpdateUserProfileSettingsRequest(
    bool IsPublicProfileEnabled,
    bool IsWishlistPublic,
    bool IsStatsPublic,
    bool IsRecentActivityPublic,
    string? FavoriteQuoteOrHeadline);
