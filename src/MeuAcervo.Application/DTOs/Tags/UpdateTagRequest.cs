namespace MeuAcervo.Application.DTOs.Tags;

public sealed record UpdateTagRequest(
    string Name,
    string? Color,
    string? Description);
