using System.Security.Claims;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Shared.Auth;

namespace MeuAcervo.API.Auth;

public sealed class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId => GetGuidClaim(ClaimTypes.NameIdentifier) ?? GetGuidClaim("sub");

    public Guid? TenantId => GetGuidClaim(JwtClaimNames.TenantId);

    public string? Email => GetClaim(ClaimTypes.Email) ?? GetClaim("email");

    public string? Username => GetClaim(JwtClaimNames.Username) ?? GetClaim(ClaimTypes.Name);

    public IReadOnlyCollection<string> Roles => GetRoles();

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsInRole(string role)
    {
        return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    private Guid? GetGuidClaim(string claimType)
    {
        var claimValue = GetClaim(claimType);
        return Guid.TryParse(claimValue, out var parsedValue) ? parsedValue : null;
    }

    private string? GetClaim(string claimType)
    {
        return Principal?.FindFirstValue(claimType);
    }

    private IReadOnlyCollection<string> GetRoles()
    {
        if (Principal is null)
        {
            return Array.Empty<string>();
        }

        var roles = Principal.FindAll(JwtClaimNames.Roles)
            .SelectMany(claim => claim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Concat(Principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return roles;
    }
}
