# Performance

## Objetivo

Garantir boa experiência mesmo com acervos grandes, múltiplos filtros e integrações externas.

## Diretrizes principais

- paginação obrigatória em listagens
- filtros indexáveis
- consultas tenant-scoped eficientes
- deduplicação de busca externa com custo controlado
- evitar `Include` excessivo

## Índices recomendados

### `UserLibraryItem`

- `TenantId, UserId, ShelfType`
- `TenantId, UserId, ReadingStatus`
- `TenantId, IsFavorite`
- `BookEditionId`
- `UpdatedAtUtc`

### `Tag`

- `TenantId, Slug`

### `Review`

- `TenantId, UserId`
- `TenantId, Visibility`

### `Loan`

- `TenantId, Status`
- `TenantId, DueAtUtc`

### `CustomFieldDefinition`

- `TenantId, EntityType, Key`

### `CustomFieldValue`

- `TenantId, EntityType, EntityId`

### `BookEdition`

- `Isbn13`
- `Isbn10`
- `BookWorkId`
- `PublisherId`
- `NormalizedTitle, Language`

### `BookWork`

- `NormalizedCanonicalTitle`
- `PrimaryLanguage`

### `Author`

- `NormalizedName`

### `Publisher`

- `NormalizedName`

### `ExternalBookReference`

- `Provider, ExternalId, ReferenceType`
- `BookWorkId`
- `BookEditionId`

## Estratégias recomendadas

- projeções com `Select` para DTOs
- paginação por offset na v1
- considerar keyset pagination no futuro para feeds e atividades
- cache de buscas externas
- cache leve para permissões e metadados estáveis

## Busca externa

- executar providers em paralelo
- aplicar timeout
- evitar persistência desnecessária de resultados transitórios

## Postgres

Usar:

- índices compostos
- constraints de unicidade
- `jsonb` somente onde agregar valor real
- paginação com cuidado para evitar custo excessivo em offsets muito altos
- índices parciais para ISBN quando os valores forem opcionais

## Observabilidade

Monitorar no futuro:

- tempo de resposta por endpoint
- tempo por provider externo
- queries lentas
- volume de itens por tenant
