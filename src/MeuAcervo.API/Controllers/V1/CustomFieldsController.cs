using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.API.Auth;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.CustomFields;
using MeuAcervo.Application.Services.CustomFields;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[Authorize]
[Route("api/v1/custom-fields")]
public sealed class CustomFieldsController : ApiControllerBase
{
    private readonly ICustomFieldService _customFieldService;
    private readonly ICurrentUserContext _currentUserContext;

    public CustomFieldsController(ICustomFieldService customFieldService, ICurrentUserContext currentUserContext)
    {
        _customFieldService = customFieldService;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CanReadCustomFields)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<CustomFieldDefinitionResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CustomFieldDefinitionResponse>>>> GetDefinitions(
        [FromQuery] GetCustomFieldDefinitionsRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenantId();
        var response = await _customFieldService.GetDefinitionsAsync(tenantId, request, cancellationToken);
        return OkResponse(response);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanManageCustomFields)]
    [ProducesResponseType(typeof(ApiResponse<CustomFieldDefinitionResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CustomFieldDefinitionResponse>>> Create(
        [FromBody] CreateCustomFieldDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenantId();
        var response = await _customFieldService.CreateDefinitionAsync(tenantId, request, cancellationToken);
        return StatusCodeResponse(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageCustomFields)]
    [ProducesResponseType(typeof(ApiResponse<CustomFieldDefinitionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CustomFieldDefinitionResponse>>> Update(
        Guid id,
        [FromBody] UpdateCustomFieldDefinitionRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenantId();
        var response = await _customFieldService.UpdateDefinitionAsync(tenantId, id, request, cancellationToken);
        return OkResponse(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageCustomFields)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenantId();
        await _customFieldService.DeleteDefinitionAsync(tenantId, id, cancellationToken);
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
