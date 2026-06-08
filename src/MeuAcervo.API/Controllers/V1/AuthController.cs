using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Auth;
using MeuAcervo.Application.Services.Auth;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[Route("api/v1/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserContext _currentUserContext;

    public AuthController(IAuthService authService, ICurrentUserContext currentUserContext)
    {
        _authService = authService;
        _currentUserContext = currentUserContext;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthSessionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AuthSessionResponse>>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, GetClientIpAddress(), cancellationToken);
        return StatusCodeResponse(StatusCodes.Status201Created, response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthSessionResponse>>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, GetClientIpAddress(), cancellationToken);
        return OkResponse(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthSessionResponse>>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshAsync(request, GetClientIpAddress(), cancellationToken);
        return OkResponse(response);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<LogoutResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LogoutResponse>>> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        EnsureAuthenticatedContext();

        var response = await _authService.LogoutAsync(
            _currentUserContext.UserId!.Value,
            _currentUserContext.TenantId!.Value,
            request,
            cancellationToken);

        return OkResponse(response);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<CurrentSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<CurrentSessionResponse>>> Me(CancellationToken cancellationToken)
    {
        EnsureAuthenticatedContext();

        var response = await _authService.GetCurrentSessionAsync(
            _currentUserContext.UserId!.Value,
            _currentUserContext.TenantId!.Value,
            cancellationToken);

        return OkResponse(response);
    }

    private void EnsureAuthenticatedContext()
    {
        if (!_currentUserContext.UserId.HasValue || !_currentUserContext.TenantId.HasValue)
        {
            throw new UnauthorizedException("Authenticated context is incomplete.");
        }
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
