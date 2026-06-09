namespace MeuAcervo.Application.DTOs.Profiles;

public sealed record UserProfileSettingsResponse(
    Guid? Id,
    string Username,
    string DisplayName,
    bool IsPublicProfileEnabled,
    bool IsWishlistPublic,
    bool IsStatsPublic,
    bool IsRecentActivityPublic,
    string? FavoriteQuoteOrHeadline,
    DateTime UpdatedAtUtc);
