# Tags

## Objetivo

Permitir classificação livre e personalizada dos itens do acervo.

## Escopo

Tags são tenant-scoped e pertencem ao usuário ou tenant atual.

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
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restrição:

- unicidade de `TenantId + Slug`

### UserLibraryItemTag

Tabela de relacionamento N:N entre item e tag.

## Regras de domínio

- nome de tag deve ser único por tenant após normalização
- tags excluídas logicamente não aparecem em novas associações
- um item não pode receber a mesma tag duas vezes

## Casos de uso

- organizar leitura por gênero pessoal
- marcar coleção especial
- marcar prioridade
- separar livros emprestados, relidos ou favoritos temáticos

## API sugerida

- `GET /api/v1/tags`
- `POST /api/v1/tags`
- `PUT /api/v1/tags/{id}`
- `DELETE /api/v1/tags/{id}`
- `POST /api/v1/library-items/{id}/tags`
- `DELETE /api/v1/library-items/{id}/tags/{tagId}`

## Exposição pública

Tags só devem aparecer em endpoints públicos se forem consideradas parte visível do perfil e se isso for explicitamente permitido pelo produto.
