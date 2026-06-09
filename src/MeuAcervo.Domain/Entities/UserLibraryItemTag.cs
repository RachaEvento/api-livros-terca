using MeuAcervo.Domain.Common;

namespace MeuAcervo.Domain.Entities;

public sealed class UserLibraryItemTag : TenantScopedAuditableEntityBase
{
    public Guid UserLibraryItemId { get; set; }

    public Guid TagId { get; set; }

    public UserLibraryItem? UserLibraryItem { get; set; }

    public Tag? Tag { get; set; }
}
