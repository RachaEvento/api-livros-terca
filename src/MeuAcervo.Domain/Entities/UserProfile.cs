using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class UserProfile : TenantScopedAuditableEntityBase
{
    public Guid UserId { get; set; }

    public bool IsPublicProfileEnabled { get; set; }

    public bool IsWishlistPublic { get; set; }

    public bool IsStatsPublic { get; set; } = true;

    public bool IsRecentActivityPublic { get; set; }

    public string? FavoriteQuoteOrHeadline { get; set; }

    public User? User { get; set; }
}
