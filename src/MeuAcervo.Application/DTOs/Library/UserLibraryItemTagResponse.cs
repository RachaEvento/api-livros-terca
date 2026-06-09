namespace MeuAcervo.Application.DTOs.Library;

public sealed record UserLibraryItemTagResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Color);
