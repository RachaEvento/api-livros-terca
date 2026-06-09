using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.API.Auth;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Reviews;
using MeuAcervo.Application.Services.Reviews;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[Authorize]
[Route("api/v1/library-items/{libraryItemId:guid}/review")]
public sealed class LibraryItemReviewController : ApiControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ICurrentUserContext _currentUserContext;

    public LibraryItemReviewController(IReviewService reviewService, ICurrentUserContext currentUserContext)
    {
        _reviewService = reviewService;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CanReadReviews)]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> Get(Guid libraryItemId, CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _reviewService.GetByLibraryItemAsync(tenantId, userId, libraryItemId, cancellationToken);
        return OkResponse(response);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanWriteReviews)]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> Create(
        Guid libraryItemId,
        [FromBody] UpsertReviewRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _reviewService.CreateAsync(tenantId, userId, libraryItemId, request, cancellationToken);
        return StatusCodeResponse(StatusCodes.Status201Created, response);
    }

    [HttpPut]
    [Authorize(Policy = AuthorizationPolicies.CanWriteReviews)]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> Update(
        Guid libraryItemId,
        [FromBody] UpsertReviewRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _reviewService.UpdateAsync(tenantId, userId, libraryItemId, request, cancellationToken);
        return OkResponse(response);
    }

    [HttpDelete]
    [Authorize(Policy = AuthorizationPolicies.CanWriteReviews)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid libraryItemId, CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        await _reviewService.DeleteAsync(tenantId, userId, libraryItemId, cancellationToken);
        return NoContent();
    }

    private (Guid TenantId, Guid UserId) EnsureAuthenticatedContext()
    {
        if (!_currentUserContext.TenantId.HasValue || !_currentUserContext.UserId.HasValue)
        {
            throw new UnauthorizedException("Authenticated context is incomplete.");
        }

        return (_currentUserContext.TenantId.Value, _currentUserContext.UserId.Value);
    }
}
