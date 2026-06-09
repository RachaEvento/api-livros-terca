namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record UpsertLibraryItemCustomFieldValuesRequest(
    IReadOnlyCollection<CustomFieldValueInputRequest> Values);
