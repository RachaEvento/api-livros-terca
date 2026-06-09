# Reviews e Notas

## Objetivo

Permitir que o usuario registre opiniao, avaliacao e conteudo textual associado aos livros do seu acervo.

## Conceitos

- `PrivateNotes` continua pertencendo ao `UserLibraryItem`
- `Review` representa conteudo estruturado com visibilidade controlada

## Requisitos funcionais

- avaliar livro de 1 a 5 estrelas
- escrever titulo e conteudo da review
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
- `IsDeleted`
- `DeletedAtUtc`
- `CreatedAtUtc`
- `UpdatedAtUtc`

## Regras de dominio

- `Rating` deve ser inteiro de 1 a 5
- review publica exige item existente no acervo do usuario
- uma review ativa por item por usuario na v1
- nota privada pode existir sem review publica
- review e item devem pertencer ao mesmo tenant

## Visibilidade sugerida

- `Private`
- `Public`

Pode haver expansao futura para:

- `FollowersOnly`

## Impacto no perfil publico

Somente reviews com `Visibility = Public` podem aparecer em:

- perfil publico
- atividade recente publica
- listagens publicas de avaliacoes

## API sugerida

- `GET /api/v1/library-items/{id}/review`
- `POST /api/v1/library-items/{id}/review`
- `PUT /api/v1/library-items/{id}/review`
- `DELETE /api/v1/library-items/{id}/review`
- `GET /api/v1/users/{username}/reviews`

Na fase atual, o endpoint publico por `username` pode permanecer preparado para futuro se o perfil publico ainda nao estiver materializado.

## Observacao de design

Separar review de nota privada evita vazamento acidental de conteudo pessoal e suporta melhor o perfil publico.
