using MeuAcervo.Domain.Common;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Domain.Entities;

public sealed class Review : TenantScopedSoftDeletableAuditableEntityBase
{
    public Guid UserId { get; set; }

    public Guid UserLibraryItemId { get; set; }

    public int Rating { get; set; }

    public string? Title { get; set; }

    public string Content { get; set; } = string.Empty;

    public ReviewVisibility Visibility { get; set; } = ReviewVisibility.Private;

    public bool ContainsSpoilers { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    public User? User { get; set; }

    public UserLibraryItem? UserLibraryItem { get; set; }
}
