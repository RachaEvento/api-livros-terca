namespace MeuAcervo.Shared.Results;

public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    ApiError? Error,
    ApiMetadata Meta)
{
    public static ApiResponse<T> Ok(T data, ApiMetadata meta) => new(true, data, null, meta);

    public static ApiResponse<T> Fail(ApiError error, ApiMetadata meta) => new(false, default, error, meta);
}
