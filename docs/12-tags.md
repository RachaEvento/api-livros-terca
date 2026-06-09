# Tags

## Objetivo

Permitir classificacao livre e personalizada dos itens do acervo.

## Escopo

Tags sao tenant-scoped e compartilhadas dentro do tenant atual. Na v1 isso equivale ao usuario autenticado, mas o modelo deve continuar valido para tenants multiusuario no futuro.

## Requisitos funcionais

- criar tag
- editar tag
- excluir logicamente tag
- vincular tag a item do acervo
- remover tag de item
- filtrar acervo por tag

## Modelagem sugerida

### Tag

Campos:

- `Id`
- `TenantId`
- `Name`
- `Slug`
- `Color`
- `Description`
- `IsDeleted`
- `DeletedAtUtc`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restricao:

- unicidade de `TenantId + Slug` para tags ativas

### UserLibraryItemTag

Tabela de relacionamento N:N entre item e tag.

## Regras de dominio

- nome de tag deve ser unico por tenant apos normalizacao
- tags excluidas logicamente nao aparecem em novas associacoes
- um item nao pode receber a mesma tag duas vezes
- tag e item devem pertencer ao mesmo tenant

## API sugerida

- `GET /api/v1/tags`
- `POST /api/v1/tags`
- `PUT /api/v1/tags/{id}`
- `DELETE /api/v1/tags/{id}`
- `POST /api/v1/library-items/{id}/tags`
- `DELETE /api/v1/library-items/{id}/tags/{tagId}`

## Exposicao publica

Tags so devem aparecer em endpoints publicos se forem consideradas parte visivel do perfil e se isso for explicitamente permitido pelo produto.
