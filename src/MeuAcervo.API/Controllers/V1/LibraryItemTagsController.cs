using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.API.Auth;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Tags;
using MeuAcervo.Application.Services.Tags;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[Authorize]
[Route("api/v1/library-items/{libraryItemId:guid}/tags")]
public sealed class LibraryItemTagsController : ApiControllerBase
{
    private readonly ITagService _tagService;
    private readonly ICurrentUserContext _currentUserContext;

    public LibraryItemTagsController(ITagService tagService, ICurrentUserContext currentUserContext)
    {
        _tagService = tagService;
        _currentUserContext = currentUserContext;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanManageTags)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<TagResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<TagResponse>>>> Assign(
        Guid libraryItemId,
        [FromBody] AssignLibraryItemTagsRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _tagService.AssignToLibraryItemAsync(tenantId, userId, libraryItemId, request, cancellationToken);
        return OkResponse(response);
    }

    [HttpDelete("{tagId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageTags)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove(Guid libraryItemId, Guid tagId, CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        await _tagService.RemoveFromLibraryItemAsync(tenantId, userId, libraryItemId, tagId, cancellationToken);
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
