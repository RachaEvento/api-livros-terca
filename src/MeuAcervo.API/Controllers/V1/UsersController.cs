using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuAcervo.Application.DTOs.Profiles;
using MeuAcervo.Application.Services.Profiles;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Controllers.V1;

[AllowAnonymous]
[Route("api/v1/users")]
public sealed class UsersController : ApiControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UsersController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet("{username}")]
    [ProducesResponseType(typeof(ApiResponse<PublicUserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PublicUserProfileResponse>>> GetProfile(string username, CancellationToken cancellationToken)
    {
        var response = await _userProfileService.GetPublicProfileAsync(username, cancellationToken);
        return OkResponse(response);
    }

    [HttpGet("{username}/library")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PublicLibraryItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PublicLibraryItemResponse>>>> GetLibrary(
        string username,
        [FromQuery] GetPublicLibraryItemsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _userProfileService.GetPublicLibraryAsync(username, request, cancellationToken);
        return PagedOkResponse(response);
    }

    [HttpGet("{username}/favorites")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PublicFavoriteItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PublicFavoriteItemResponse>>>> GetFavorites(
        string username,
        [FromQuery] GetPublicFavoritesRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _userProfileService.GetPublicFavoritesAsync(username, request, cancellationToken);
        return PagedOkResponse(response);
    }

    [HttpGet("{username}/reviews")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PublicReviewResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PublicReviewResponse>>>> GetReviews(
        string username,
        [FromQuery] GetPublicReviewsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _userProfileService.GetPublicReviewsAsync(username, request, cancellationToken);
        return PagedOkResponse(response);
    }

    [HttpGet("{username}/activity")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PublicActivityEntryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<PublicActivityEntryResponse>>>> GetActivity(
        string username,
        [FromQuery] GetPublicActivityRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _userProfileService.GetPublicActivityAsync(username, request, cancellationToken);
        return PagedOkResponse(response);
    }
}
