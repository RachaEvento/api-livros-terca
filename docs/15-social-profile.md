# Perfil Social

## Objetivo

Disponibilizar um perfil público parcial do leitor sem exigir rede social completa na v1.

## Elementos públicos

- `Username` único
- avatar
- bio
- estatísticas de leitura
- favoritos públicos
- leituras recentes públicas
- wishlist pública opcional
- reviews públicas

## Entidade sugerida

### UserProfile

Campos:

- `UserId`
- `TenantId`
- `IsPublicProfileEnabled`
- `IsWishlistPublic`
- `IsStatsPublic`
- `IsRecentActivityPublic`
- `FavoriteQuoteOrHeadline`
- `UpdatedAtUtc`

## Estatísticas sugeridas

- quantidade total de itens
- quantidade lida
- quantidade em leitura
- nota média
- gêneros ou tags mais usadas
- leituras recentes

## Fontes de dados

As estatísticas devem ser derivadas do acervo do usuário e de dados marcados como públicos.

## Endpoints públicos sugeridos

- `GET /api/v1/users/{username}`
- `GET /api/v1/users/{username}/library`
- `GET /api/v1/users/{username}/reviews`
- `GET /api/v1/users/{username}/favorites`

## Regras de privacidade

- nada privado pode aparecer por padrão
- wishlist só aparece se configurada como pública
- custom fields só aparecem se o campo estiver marcado como público
- notas privadas nunca aparecem

## Evolução futura

- seguidores
- feed de atividade
- curtidas
- comentários em reviews
