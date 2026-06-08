# Campos Customizados

## Objetivo

Permitir que cada tenant crie metadados próprios sem exigir nova migration para cada campo novo.

## Escopo da v1

Na primeira versão, os campos customizados serão aplicados a:

- `UserLibraryItem`

O modelo deve, porém, permitir expansão futura para outras entidades.

## Tipos suportados

- `Text`
- `Number`
- `Date`
- `Boolean`
- `List`

## Requisitos funcionais

O usuário poderá:

- criar campo customizado
- definir rótulo e chave interna
- escolher tipo
- marcar como obrigatório ou opcional
- definir visibilidade pública
- ativar ou desativar
- reordenar
- manter opções para campos do tipo lista

## Modelagem recomendada

### CustomFieldDefinition

Campos sugeridos:

- `Id`
- `TenantId`
- `EntityType`
- `Key`
- `Label`
- `DataType`
- `IsRequired`
- `IsPublic`
- `IsActive`
- `SortOrder`
- `ConfigurationJson`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restrições:

- unicidade de `TenantId + EntityType + Key`

### CustomFieldOption

Campos sugeridos:

- `Id`
- `TenantId`
- `CustomFieldDefinitionId`
- `Value`
- `Label`
- `SortOrder`

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
- `JsonValue`
- `CreatedAtUtc`
- `UpdatedAtUtc`

## Estratégia de armazenamento

Usar tabela de valores tipados, evitando um único campo texto para tudo. Isso facilita:

- validação
- consulta
- indexação seletiva
- evolução do domínio

## Regras de validação

- somente um tipo de valor pode estar preenchido por registro
- campos obrigatórios devem existir ao salvar o item
- campos inativos não devem ser exigidos em novas escritas
- valor de lista deve existir entre as opções da definição

## Exposição pública

Somente campos marcados como `IsPublic` podem aparecer no perfil público ou em endpoints públicos.

## Impactos na API

Os endpoints de item do acervo devem aceitar uma coleção de valores customizados, por exemplo:

```json
[
  {
    "fieldKey": "purchase-price",
    "value": 59.90
  },
  {
    "fieldKey": "gifted-by",
    "value": "Ana"
  }
]
```

O serviço de aplicação deve resolver `fieldKey` para `CustomFieldDefinition`.

## Índices recomendados

- `TenantId, EntityType, Key` em `CustomFieldDefinition`
- `TenantId, EntityType, EntityId` em `CustomFieldValue`
- `CustomFieldDefinitionId` em `CustomFieldValue`

## Decisão de design

Foi escolhida modelagem relacional flexível em vez de migration por campo, pois isso atende melhor ao objetivo do produto e preserva governança sobre validação e consulta.
