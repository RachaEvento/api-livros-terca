# Performance

## Objetivo

Garantir boa experiencia mesmo com acervos grandes, multiplos filtros, consultas publicas e integracoes externas.

## Diretrizes principais

- paginacao obrigatoria em listagens
- filtros indexaveis
- consultas tenant-scoped eficientes
- projecoes com `Select` para endpoints publicos
- deduplicacao de busca externa com custo controlado
- evitar `Include` excessivo
- preferir `AsNoTracking` em consultas de leitura

## Indices recomendados

### `UserLibraryItem`

- `TenantId, UserId, ShelfType`
- `TenantId, UserId, ReadingStatus`
- `TenantId, UserId, IsFavorite`
- `TenantId, UserId, BookEditionId` unico parcial para itens ativos
- `TenantId, UserId, StartedAt`
- `TenantId, UserId, FinishedAt`
- `BookEditionId`
- `UpdatedAtUtc`

### `UserProfile`

- `UserId` unico
- `TenantId, IsPublicProfileEnabled`

### `Review`

- `TenantId, UserId`
- `TenantId, Visibility`
- `TenantId, UserId, Visibility, PublishedAtUtc`

### `Loan`

- `TenantId, Status`
- `TenantId, DueAtUtc`
- `TenantId, UserLibraryItemId, Status` para emprestimo ativo por item

### `CustomFieldDefinition`

- `TenantId, EntityType, SortOrder`

### `CustomFieldValue`

- `TenantId, EntityType, EntityId`
- `TenantId, EntityType, EntityId, CustomFieldDefinitionId`

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

## Estrategias recomendadas

- projecoes para DTOs publicos em vez de carregar o grafo inteiro do acervo
- paginacao por offset na v1
- considerar keyset pagination no futuro para feeds e atividades
- cache de buscas externas
- cache leve para permissoes e metadados estaveis
- consultas publicas em duas etapas quando houver custom fields publicos: primeiro ids paginados, depois enriquecimento controlado

## Busca externa

- executar providers em paralelo
- aplicar timeout
- evitar persistencia desnecessaria de resultados transitorios

## Postgres

Usar:

- indices compostos
- constraints de unicidade
- `jsonb` somente onde agregar valor real
- paginacao com cuidado para evitar custo excessivo em offsets muito altos
- indices parciais para ISBN quando os valores forem opcionais

## Consultas publicas

As consultas de perfil publico devem:

- resolver o usuario por `NormalizedUsername`
- validar a flag do perfil antes de executar consultas agregadas mais caras
- nunca reutilizar endpoints internos do acervo
- nunca materializar `PrivateNotes`, reviews privadas ou custom fields nao publicos
- paginar a biblioteca publica em duas etapas quando houver enriquecimento com custom fields ou review publica resumida

## Observabilidade

Monitorar no futuro:

- tempo de resposta por endpoint
- tempo por provider externo
- queries lentas
- volume de itens por tenant
- custo das consultas publicas por username
