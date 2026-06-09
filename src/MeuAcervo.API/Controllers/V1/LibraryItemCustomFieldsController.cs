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
[Route("api/v1/library-items/{libraryItemId:guid}/custom-fields")]
public sealed class LibraryItemCustomFieldsController : ApiControllerBase
{
    private readonly ICustomFieldService _customFieldService;
    private readonly ICurrentUserContext _currentUserContext;

    public LibraryItemCustomFieldsController(ICustomFieldService customFieldService, ICurrentUserContext currentUserContext)
    {
        _customFieldService = customFieldService;
        _currentUserContext = currentUserContext;
    }

    [HttpPut]
    [Authorize(Policy = AuthorizationPolicies.CanManageCustomFields)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<CustomFieldValueResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CustomFieldValueResponse>>>> Upsert(
        Guid libraryItemId,
        [FromBody] UpsertLibraryItemCustomFieldValuesRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _customFieldService.UpsertLibraryItemValuesAsync(tenantId, userId, libraryItemId, request, cancellationToken);
        return OkResponse(response);
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
