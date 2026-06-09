using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.API.Auth;
using MeuAcervo.Application.DTOs.Books;
using MeuAcervo.Application.Services.Books;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[Authorize]
[Route("api/v1/books")]
public sealed class BooksController : ApiControllerBase
{
    private readonly IBookCatalogService _bookCatalogService;

    public BooksController(IBookCatalogService bookCatalogService)
    {
        _bookCatalogService = bookCatalogService;
    }

    [HttpGet("search")]
    [Authorize(Policy = AuthorizationPolicies.CanReadLibrary)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<BookSearchResultResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<BookSearchResultResponse>>>> Search(
        [FromQuery] BookSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _bookCatalogService.SearchAsync(request, cancellationToken);
        return PagedOkResponse(response);
    }

}
