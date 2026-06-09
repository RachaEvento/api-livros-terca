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
[Route("api/v1/tags")]
public sealed class TagsController : ApiControllerBase
{
    private readonly ITagService _tagService;
    private readonly ICurrentUserContext _currentUserContext;

    public TagsController(ITagService tagService, ICurrentUserContext currentUserContext)
    {
        _tagService = tagService;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CanReadTags)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<TagResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<TagResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenantId();
        var response = await _tagService.GetAllAsync(tenantId, cancellationToken);
        return OkResponse(response);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanManageTags)]
    [ProducesResponseType(typeof(ApiResponse<TagResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<TagResponse>>> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenantId();
        var response = await _tagService.CreateAsync(tenantId, request, cancellationToken);
        return StatusCodeResponse(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageTags)]
    [ProducesResponseType(typeof(ApiResponse<TagResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TagResponse>>> Update(Guid id, [FromBody] UpdateTagRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenantId();
        var response = await _tagService.UpdateAsync(tenantId, id, request, cancellationToken);
        return OkResponse(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageTags)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenantId();
        await _tagService.DeleteAsync(tenantId, id, cancellationToken);
        return NoContent();
    }

    private Guid EnsureTenantId()
    {
        if (!_currentUserContext.TenantId.HasValue)
        {
            throw new UnauthorizedException("Authenticated context is incomplete.");
        }

        return _currentUserContext.TenantId.Value;
    }
}
