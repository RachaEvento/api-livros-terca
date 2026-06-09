using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class Tag : TenantScopedSoftDeletableAuditableEntityBase
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Color { get; set; }

    public string? Description { get; set; }

    public ICollection<UserLibraryItemTag> UserLibraryItemTags { get; set; } = new List<UserLibraryItemTag>();
}
