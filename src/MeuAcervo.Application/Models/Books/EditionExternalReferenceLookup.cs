namespace MeuAcervo.Application.Models.Books;

public sealed record EditionExternalReferenceLookup(
    string Provider,
    string ExternalId);
