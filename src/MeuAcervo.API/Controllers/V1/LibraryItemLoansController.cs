using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.API.Auth;
using MeuAcervo.Application.Abstractions.CurrentUser;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Loans;
using MeuAcervo.Application.Services.Loans;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[Authorize]
[Route("api/v1/library-items/{libraryItemId:guid}/loans")]
public sealed class LibraryItemLoansController : ApiControllerBase
{
    private readonly ILoanService _loanService;
    private readonly ICurrentUserContext _currentUserContext;

    public LibraryItemLoansController(ILoanService loanService, ICurrentUserContext currentUserContext)
    {
        _loanService = loanService;
        _currentUserContext = currentUserContext;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanManageLoans)]
    [ProducesResponseType(typeof(ApiResponse<LoanResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<LoanResponse>>> Create(
        Guid libraryItemId,
        [FromBody] CreateLoanRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _loanService.CreateAsync(tenantId, userId, libraryItemId, request, cancellationToken);
        return StatusCodeResponse(StatusCodes.Status201Created, response);
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
