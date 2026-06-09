using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class User : TenantScopedAuditableEntityBase
{
    public string Email { get; set; } = string.Empty;

    public string NormalizedEmail { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string NormalizedUsername { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public bool IsActive { get; set; } = true;

    public Tenant? Tenant { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<UserLibraryItem> UserLibraryItems { get; set; } = new List<UserLibraryItem>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
