namespace MeuAcervo.Application.Models.Profiles;

public sealed record PublicProfileLookup(
    Guid TenantId,
    Guid UserId,
    string Username,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    string? FavoriteQuoteOrHeadline,
    bool IsPublicProfileEnabled,
    bool IsWishlistPublic,
    bool IsStatsPublic,
    bool IsRecentActivityPublic);
