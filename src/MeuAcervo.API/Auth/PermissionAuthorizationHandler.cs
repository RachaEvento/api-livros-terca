using Microsoft.AspNetCore.Authorization;
using MeuAcervo.Application.Abstractions.Auth;
using MeuAcervo.Application.Abstractions.CurrentUser;

namespace MeuAcervo.API.Auth;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IIdentityRepository _identityRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionAuthorizationHandler(
        ICurrentUserContext currentUserContext,
        IIdentityRepository identityRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _currentUserContext = currentUserContext;
        _identityRepository = identityRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (!_currentUserContext.IsAuthenticated || !_currentUserContext.UserId.HasValue || !_currentUserContext.TenantId.HasValue)
        {
            return;
        }

        var cancellationToken = _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        var hasPermission = await _identityRepository.UserHasPermissionAsync(
            _currentUserContext.UserId.Value,
            _currentUserContext.TenantId.Value,
            requirement.PermissionCode,
            cancellationToken);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
