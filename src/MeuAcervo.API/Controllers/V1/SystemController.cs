using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.Application.DTOs.System;
using MeuAcervo.Application.Services.System;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[AllowAnonymous]
[Route("api/v1/system")]
public sealed class SystemController : ApiControllerBase
{
    private readonly ISystemInfoService _systemInfoService;

    public SystemController(ISystemInfoService systemInfoService)
    {
        _systemInfoService = systemInfoService;
    }

    [HttpGet("info")]
    [ProducesResponseType(typeof(ApiResponse<ApiInfoResponse>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<ApiInfoResponse>> GetInfo()
    {
        var response = _systemInfoService.GetApiInfo();
        return OkResponse(response);
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(ApiResponse<HealthCheckResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<HealthCheckResponse>), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ApiResponse<HealthCheckResponse>>> GetHealth(CancellationToken cancellationToken)
    {
        var response = await _systemInfoService.GetHealthAsync(cancellationToken);
        var statusCode = response.DatabaseAvailable
            ? StatusCodes.Status200OK
            : StatusCodes.Status503ServiceUnavailable;

        return StatusCodeResponse(statusCode, response);
    }
}
