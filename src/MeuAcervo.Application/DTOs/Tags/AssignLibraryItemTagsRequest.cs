namespace MeuAcervo.Application.DTOs.Tags;

public sealed record AssignLibraryItemTagsRequest(
    IReadOnlyCollection<Guid> TagIds);
