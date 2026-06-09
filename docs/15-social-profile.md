# Perfil Social

## Objetivo

Disponibilizar um perfil publico parcial do leitor sem exigir rede social completa na v1.

## Elementos publicos

- `Username` unico
- display name
- avatar
- bio
- headline opcional
- estatisticas publicas de leitura
- favoritos publicos
- reviews publicas
- atividade recente publica quando habilitada
- wishlist publica opcional

## Entidade de configuracao

### UserProfile

Campos:

- `Id`
- `UserId`
- `TenantId`
- `IsPublicProfileEnabled`
- `IsWishlistPublic`
- `IsStatsPublic`
- `IsRecentActivityPublic`
- `FavoriteQuoteOrHeadline`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Regras:

- o perfil nasce privado por padrao
- a wishlist nasce privada por padrao
- a configuracao deve poder ser lida e alterada pelo proprio usuario autenticado
- a ausencia de `UserProfile` para usuarios legados deve ser tratada como perfil privado ate que o usuario salve a configuracao

## Estatisticas publicas da v1

- `collectionItemCount`
- `completedItemCount`
- `readingItemCount`
- `favoriteItemCount`
- `publicReviewCount`
- `averagePublicRating`
- `wishlistItemCount` apenas quando `IsWishlistPublic = true`

As estatisticas devem ser derivadas do acervo do usuario e de dados marcados como publicos.

## Atividade recente publica da v1

Para manter privacidade e baixo custo operacional, a v1 publica expoe apenas eventos seguros:

- item iniciado, quando `StartedAt` estiver preenchido
- item concluido, quando `FinishedAt` estiver preenchido
- review publicada ou atualizada, quando `Visibility = Public`

A v1 nao expoe um feed completo da biblioteca publica.

## Endpoints da v1

### Configuracao autenticada

- `GET /api/v1/profile`
- `PUT /api/v1/profile`

### Endpoints publicos

- `GET /api/v1/users/{username}`
- `GET /api/v1/users/{username}/library`
- `GET /api/v1/users/{username}/favorites`
- `GET /api/v1/users/{username}/reviews`
- `GET /api/v1/users/{username}/activity`

## Regras de privacidade

- nada privado pode aparecer por padrao
- se `IsPublicProfileEnabled = false`, nenhum endpoint publico do usuario deve responder com os dados do perfil
- wishlist so aparece publicamente quando `IsWishlistPublic = true`
- estatisticas so aparecem quando `IsStatsPublic = true`
- atividade recente so aparece quando `IsRecentActivityPublic = true`
- custom fields so podem aparecer publicamente se o campo estiver marcado como publico
- notas privadas nunca aparecem
- reviews privadas nunca aparecem
- favoritos em wishlist privada nao podem aparecer nos endpoints publicos

## Modelagem de respostas publicas

As respostas publicas devem usar DTOs dedicados e nunca reutilizar diretamente respostas internas do acervo com campos privados omitidos ad hoc.

Campos recomendados para itens publicos:

- `BookWork` e `BookEdition` em formato resumido
- autores e editora quando disponiveis
- status e datas publicas relevantes
- custom fields publicos somente quando existirem

## Biblioteca publica paginada da v1

O endpoint `GET /api/v1/users/{username}/library` deve:

- respeitar `IsPublicProfileEnabled`
- incluir itens de `Collection` por padrao
- incluir itens de `Wishlist` somente quando `IsWishlistPublic = true`
- nao expor itens `Archived` na v1 publica
- aceitar paginação por `pageNumber` e `pageSize`
- aceitar filtros publicos seguros como `shelfType`, `readingStatus`, `isFavorite`, `acquisitionFormat`, `author`, `title` e `search`
- aceitar ordenacao publica por `title`, `author`, `createdAt`, `updatedAt`, `progress` e `finishedAt`

Campos publicos recomendados por item:

- ids de `UserLibraryItem`, `BookEdition` e `BookWork`
- titulo da obra e da edicao
- autores
- editora
- idioma
- capa
- shelf publico
- status de leitura
- formato de aquisicao
- favorito
- progresso
- quantidade de leituras
- datas publicas relevantes
- review publica resumida quando existir
- custom fields publicos quando existirem

Campos que nunca devem sair nesse endpoint:

- `PrivateNotes`
- `PhysicalLocation`
- `Condition`
- custom fields nao publicos
- reviews privadas

## Evolucao futura

- biblioteca publica paginada
- seguidores
- feed de atividade completo
- curtidas
- comentarios em reviews
