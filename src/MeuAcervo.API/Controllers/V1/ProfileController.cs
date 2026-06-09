using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.API.Auth;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Profiles;
using MeuAcervo.Application.Services.Profiles;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[Authorize]
[Route("api/v1/profile")]
public sealed class ProfileController : ApiControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly ICurrentUserContext _currentUserContext;

    public ProfileController(IUserProfileService userProfileService, ICurrentUserContext currentUserContext)
    {
        _userProfileService = userProfileService;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CanReadProfile)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileSettingsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserProfileSettingsResponse>>> GetCurrent(CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _userProfileService.GetCurrentAsync(tenantId, userId, cancellationToken);
        return OkResponse(response);
    }

    [HttpPut]
    [Authorize(Policy = AuthorizationPolicies.CanWriteProfile)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileSettingsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserProfileSettingsResponse>>> Update(
        [FromBody] UpdateUserProfileSettingsRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _userProfileService.UpdateCurrentAsync(tenantId, userId, request, cancellationToken);
        return OkResponse(response);
    }

    private (Guid TenantId, Guid UserId) EnsureAuthenticatedContext()
    {
        if (!_currentUserContext.UserId.HasValue || !_currentUserContext.TenantId.HasValue)
        {
            throw new UnauthorizedException("Authenticated context is incomplete.");
        }

        return (_currentUserContext.TenantId.Value, _currentUserContext.UserId.Value);
    }
}
