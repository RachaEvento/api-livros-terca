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
[Route("api/v1/loans")]
public sealed class LoansController : ApiControllerBase
{
    private readonly ILoanService _loanService;
    private readonly ICurrentUserContext _currentUserContext;

    public LoansController(ILoanService loanService, ICurrentUserContext currentUserContext)
    {
        _loanService = loanService;
        _currentUserContext = currentUserContext;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CanReadLoans)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<LoanResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<LoanResponse>>>> GetAll(
        [FromQuery] GetLoansRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _loanService.GetLoansAsync(tenantId, userId, request, cancellationToken);
        return PagedOkResponse(response);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CanReadLoans)]
    [ProducesResponseType(typeof(ApiResponse<LoanResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LoanResponse>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _loanService.GetByIdAsync(tenantId, userId, id, cancellationToken);
        return OkResponse(response);
    }

    [HttpPatch("{id:guid}/return")]
    [Authorize(Policy = AuthorizationPolicies.CanManageLoans)]
    [ProducesResponseType(typeof(ApiResponse<LoanResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LoanResponse>>> Return(Guid id, [FromBody] ReturnLoanRequest request, CancellationToken cancellationToken)
    {
        var (tenantId, userId) = EnsureAuthenticatedContext();
        var response = await _loanService.ReturnAsync(tenantId, userId, id, request, cancellationToken);
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
