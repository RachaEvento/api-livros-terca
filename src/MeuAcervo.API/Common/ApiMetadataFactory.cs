using MeuAcervo.Shared.Pagination;
using MeuAcervo.Shared.Results;

namespace MeuAcervo.API.Common;

public static class ApiMetadataFactory
{
    public static ApiMetadata Create(HttpContext httpContext)
    {
        return new ApiMetadata(httpContext.TraceIdentifier);
    }

    public static ApiMetadata Create<T>(HttpContext httpContext, PagedResult<T> pagedResult)
    {
        return new ApiMetadata(
            httpContext.TraceIdentifier,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages);
    }
}
