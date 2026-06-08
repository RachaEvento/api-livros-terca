using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.API.Auth;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Library;
using MeuAcervo.Application.Services.Library;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[Authorize]
[Route("api/v1/library-items")]
public sealed class LibraryItemsController : ApiControllerBase
{
    private readonly IUserLibraryService _userLibraryService;
    private readonly ICurrentUserContext _currentUserContext;

    public LibraryItemsController(IUserLibraryService userLibraryService, ICurrentUserContext currentUserContext)
    {
        _userLibraryService = userLibraryService;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CanReadLibrary)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<UserLibraryItemListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<UserLibraryItemListResponse>>>> GetItems(
        [FromQuery] GetUserLibraryItemsRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _userLibraryService.GetItemsAsync(tenantId, userId, request, cancellationToken);
        return PagedOkResponse(response);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanWriteLibrary)]
    [ProducesResponseType(typeof(ApiResponse<UserLibraryItemDetailResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserLibraryItemDetailResponse>>> Create(
        [FromBody] CreateUserLibraryItemRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _userLibraryService.CreateItemAsync(tenantId, userId, request, cancellationToken);
        return StatusCodeResponse(StatusCodes.Status201Created, response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanReadLibrary)]
    [ProducesResponseType(typeof(ApiResponse<UserLibraryItemDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserLibraryItemDetailResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _userLibraryService.GetItemByIdAsync(tenantId, userId, id, cancellationToken);
        return OkResponse(response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanWriteLibrary)]
    [ProducesResponseType(typeof(ApiResponse<UserLibraryItemDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserLibraryItemDetailResponse>>> Update(
        Guid id,
        [FromBody] UpdateUserLibraryItemRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _userLibraryService.UpdateItemAsync(tenantId, userId, id, request, cancellationToken);
        return OkResponse(response);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = AuthorizationPolicies.CanWriteLibrary)]
    [ProducesResponseType(typeof(ApiResponse<UserLibraryItemDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserLibraryItemDetailResponse>>> UpdateStatus(
        Guid id,
        [FromBody] UpdateUserLibraryItemStatusRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _userLibraryService.UpdateStatusAsync(tenantId, userId, id, request, cancellationToken);
        return OkResponse(response);
    }

    [HttpPost("{id:guid}/progress")]
    [Authorize(Policy = AuthorizationPolicies.CanWriteLibrary)]
    [ProducesResponseType(typeof(ApiResponse<UserLibraryItemDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserLibraryItemDetailResponse>>> RegisterProgress(
        Guid id,
        [FromBody] RegisterReadingProgressRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _userLibraryService.RegisterProgressAsync(tenantId, userId, id, request, cancellationToken);
        return OkResponse(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanWriteLibrary)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        await _userLibraryService.DeleteItemAsync(tenantId, userId, id, cancellationToken);
        return NoContent();
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
