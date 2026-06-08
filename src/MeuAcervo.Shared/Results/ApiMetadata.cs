namespace MeuAcervo.Shared.Results;

public sealed record ApiMetadata(
    string TraceId,
    int? PageNumber = null,
    int? PageSize = null,
    int? TotalCount = null,
    int? TotalPages = null);
