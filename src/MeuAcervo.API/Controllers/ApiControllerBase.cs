using Microsoft.AspNetCore.Mvc;
using MeuAcervo.API.Common;
using MeuAcervo.Shared.Pagination;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data)
    {
        return Ok(ApiResponse<T>.Ok(data, ApiMetadataFactory.Create(HttpContext)));
    }

    protected ActionResult<ApiResponse<T>> StatusCodeResponse<T>(int statusCode, T data)
    {
        return StatusCode(statusCode, ApiResponse<T>.Ok(data, ApiMetadataFactory.Create(HttpContext)));
    }

    protected ActionResult<ApiResponse<IReadOnlyCollection<T>>> PagedOkResponse<T>(PagedResult<T> pagedResult)
    {
        return Ok(ApiResponse<IReadOnlyCollection<T>>.Ok(pagedResult.Items, ApiMetadataFactory.Create(HttpContext, pagedResult)));
    }
}
