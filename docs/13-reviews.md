# Reviews e Notas

## Objetivo

Permitir que o usuário registre opinião, avaliação e conteúdo textual associado aos livros do seu acervo.

## Conceitos

- **nota privada**: observação pessoal mantida no item do acervo
- **review**: conteúdo estruturado com possibilidade de visibilidade pública

## Requisitos funcionais

- avaliar livro de 1 a 5 estrelas
- escrever título e conteúdo da review
- definir visibilidade
- marcar spoilers
- editar review
- remover review

## Modelagem sugerida

### Review

Campos:

- `Id`
- `TenantId`
- `UserId`
- `UserLibraryItemId`
- `Rating`
- `Title`
- `Content`
- `Visibility`
- `ContainsSpoilers`
- `PublishedAtUtc`
- `CreatedAtUtc`
- `UpdatedAtUtc`

## Regras de domínio

- `Rating` deve ser inteiro de 1 a 5
- review pública exige item existente no acervo do usuário
- uma review ativa por item por usuário na v1
- nota privada pode existir sem review pública

## Visibilidade sugerida

- `Private`
- `Public`

Pode haver expansão futura para:

- `FollowersOnly`

## Impacto no perfil público

Somente reviews com `Visibility = Public` podem aparecer em:

- perfil público
- atividade recente pública
- listagens públicas de avaliações

## API sugerida

- `POST /api/v1/library-items/{id}/review`
- `PUT /api/v1/library-items/{id}/review`
- `DELETE /api/v1/library-items/{id}/review`
- `GET /api/v1/users/{username}/reviews`

## Observação de design

Separar review de nota privada evita vazamento acidental de conteúdo pessoal e suporta melhor o perfil público.
