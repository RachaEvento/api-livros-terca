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
- definir rotulo
- escolher tipo
- definir visibilidade publica
- ativar ou desativar
- manter opcoes para campos do tipo lista
- atribuir valores customizados a itens do acervo

## Modelagem recomendada

### CustomFieldDefinition

Campos sugeridos:

- `Id`
- `TenantId`
- `EntityType`
- `Label`
- `DataType`
- `IsPublic`
- `IsActive`
- `SortOrder`
- `IsDeleted`
- `DeletedAtUtc`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Observacao:

- `SortOrder` e gerenciado internamente pelo backend e nao faz parte do contrato de entrada ou saida da API

Restricoes:

- identificacao e referencia por `Id`

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

Observacao:

- a ordem das opcoes e definida pela sequencia enviada no payload, mas o backend persiste essa ordem internamente sem expor `SortOrder` na API

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
- campos customizados sao sempre opcionais
- a ausencia de valor para um campo nao invalida o item do acervo
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
    "definitionId": "11111111-1111-1111-1111-111111111111",
    "numberValue": 59.90
  },
  {
    "definitionId": "22222222-2222-2222-2222-222222222222",
    "textValue": "Ana"
  },
  {
    "definitionId": "33333333-3333-3333-3333-333333333333",
    "optionValue": "gift"
  }
]
```

O servico de aplicacao deve validar `definitionId` e carregar a `CustomFieldDefinition` correspondente no tenant autenticado.

Para `CustomFieldDefinition`, o payload pode omitir `options` quando `dataType` for diferente de `List`.
Nesses casos, o backend deve tratar a colecao como vazia.

## Endpoints recomendados da fase

- `GET /api/v1/custom-fields`
- `POST /api/v1/custom-fields`
- `PUT /api/v1/custom-fields/{id}`
- `DELETE /api/v1/custom-fields/{id}`
- `PUT /api/v1/library-items/{id}/custom-fields`

## Indices recomendados

- `TenantId, EntityType, SortOrder` em `CustomFieldDefinition`
- `TenantId, EntityType, EntityId` em `CustomFieldValue`
- `CustomFieldDefinitionId` em `CustomFieldValue`

## Decisao de design

Foi escolhida modelagem relacional flexivel em vez de migration por campo, pois isso atende melhor ao objetivo do produto e preserva governanca sobre validacao e consulta.
