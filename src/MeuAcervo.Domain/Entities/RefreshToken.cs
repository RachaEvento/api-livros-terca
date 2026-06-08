using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class RefreshToken : TenantScopedAuditableEntityBase
{
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public string JwtId { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public string? CreatedByIp { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public User? User { get; set; }
}
