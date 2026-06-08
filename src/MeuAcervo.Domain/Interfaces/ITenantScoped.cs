namespace MeuAcervo.Domain.Interfaces;

public interface ITenantScoped
{
    Guid TenantId { get; set; }
}
