namespace MeuAcervo.Application.Abstractions.CurrentUser;

public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    Guid? TenantId { get; }

    string? Email { get; }

    string? Username { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsInRole(string role);
}
