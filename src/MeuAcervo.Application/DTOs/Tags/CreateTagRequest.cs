namespace MeuAcervo.Application.DTOs.Tags;

public sealed record CreateTagRequest(
    string Name,
    string? Color,
    string? Description);
