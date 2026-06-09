# Campos Customizados

## Objetivo

Permitir que cada tenant crie metadados proprios sem exigir nova migration para cada campo novo.

## Escopo da v1

Na primeira versao, os campos customizados serao aplicados a:

- `UserLibraryItem`

O modelo deve, porem, permitir expansao futura para outras entidades.

## Tipos suportados

- `Text`
- `Number`
- `Date`
- `Boolean`
- `List`

Na v1, `List` representa selecao simples de uma opcao definida pelo tenant.

## Requisitos funcionais

O usuario podera:

- criar campo customizado
- definir rotulo e chave interna
- escolher tipo
- marcar como obrigatorio ou opcional
- definir visibilidade publica
- ativar ou desativar
- reordenar
- manter opcoes para campos do tipo lista
- atribuir valores customizados a itens do acervo

## Modelagem recomendada

### CustomFieldDefinition

Campos sugeridos:

- `Id`
- `TenantId`
- `EntityType`
- `Key`
- `NormalizedKey`
- `Label`
- `DataType`
- `IsRequired`
- `IsPublic`
- `IsActive`
- `SortOrder`
- `ConfigurationJson`
- `IsDeleted`
- `DeletedAtUtc`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restricoes:

- unicidade de `TenantId + EntityType + NormalizedKey` para definicoes ativas

### CustomFieldOption

Campos sugeridos:

- `Id`
- `TenantId`
- `CustomFieldDefinitionId`
- `Value`
- `Label`
- `SortOrder`
- `CreatedAtUtc`
- `UpdatedAtUtc`

### CustomFieldValue

Campos sugeridos:

- `Id`
- `TenantId`
- `EntityType`
- `EntityId`
- `CustomFieldDefinitionId`
- `TextValue`
- `NumberValue`
- `DateValue`
- `BooleanValue`
- `OptionValue`
- `CreatedAtUtc`
- `UpdatedAtUtc`

## Estrategia de armazenamento

Usar tabela de valores tipados, evitando um unico campo texto para tudo. Isso facilita:

- validacao
- consulta
- indexacao seletiva
- evolucao do dominio

## Regras de validacao

- somente um tipo de valor pode estar preenchido por registro
- campos obrigatorios devem existir em operacoes explicitas de atribuicao de custom fields
- campos inativos nao devem ser exigidos em novas escritas
- valor de lista deve existir entre as opcoes da definicao
- a definicao deve pertencer ao mesmo tenant do item do acervo

## Exposicao publica

Somente campos marcados como `IsPublic` podem aparecer no perfil publico ou em endpoints publicos.

## Impactos na API

Os valores customizados devem usar contrato tipado, por exemplo:

```json
[
  {
    "fieldKey": "purchase-price",
    "numberValue": 59.90
  },
  {
    "fieldKey": "gifted-by",
    "textValue": "Ana"
  },
  {
    "fieldKey": "reading-origin",
    "optionValue": "gift"
  }
]
```

O servico de aplicacao deve resolver `fieldKey` para `CustomFieldDefinition`.

## Endpoints recomendados da fase

- `GET /api/v1/custom-fields`
- `POST /api/v1/custom-fields`
- `PUT /api/v1/custom-fields/{id}`
- `DELETE /api/v1/custom-fields/{id}`
- `PUT /api/v1/library-items/{id}/custom-fields`

## Indices recomendados

- `TenantId, EntityType, NormalizedKey` em `CustomFieldDefinition`
- `TenantId, EntityType, EntityId` em `CustomFieldValue`
- `CustomFieldDefinitionId` em `CustomFieldValue`

## Decisao de design

Foi escolhida modelagem relacional flexivel em vez de migration por campo, pois isso atende melhor ao objetivo do produto e preserva governanca sobre validacao e consulta.
