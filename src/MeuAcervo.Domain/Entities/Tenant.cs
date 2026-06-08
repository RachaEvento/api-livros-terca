using MeuAcervo.Domain.Common;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Domain.Entities;

public sealed class Tenant : AuditableEntityBase
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public TenantType Type { get; set; } = TenantType.Personal;

    public ICollection<User> Users { get; set; } = new List<User>();

    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
