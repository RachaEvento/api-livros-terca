namespace MeuAcervo.Shared.Pagination;

public class PagedRequest
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? SortBy { get; init; }

    public string? SortDirection { get; init; }
}
