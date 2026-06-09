using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.DTOs.CustomFields;

public sealed record GetCustomFieldDefinitionsRequest(
    CustomFieldEntityType? EntityType,
    bool IncludeInactive = false);
