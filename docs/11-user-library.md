# Acervo do Usuario

## Objetivo

Representar a experiencia principal do produto: a relacao do usuario com livros, formatos, leitura, organizacao e historico.

## Entidade central

- `UserLibraryItem`

## Capacidades obrigatorias

- adicionar item ao acervo
- adicionar item a wishlist
- atualizar status de leitura
- registrar progresso
- favoritar
- informar formato
- informar localizacao fisica
- informar estado de conservacao
- registrar quantidade de leituras
- armazenar notas privadas

## Campos de negocio recomendados

- `ShelfType`
- `ReadingStatus`
- `AcquisitionFormat`
- `OwnershipType`
- `IsFavorite`
- `CurrentPage`
- `ProgressPercent`
- `ReadCount`
- `StartedAt`
- `FinishedAt`
- `AcquiredAt`
- `PhysicalLocation`
- `Condition`
- `PrivateNotes`

## Regras de dominio

- item em `Wishlist` pode existir sem progresso
- item `Completed` deve ter `FinishedAt` ou progresso final compativel
- item `Reading` pode gerar multiplos registros em `ReadingProgressEntry`
- `ReadCount` aumenta quando o status e concluido novamente
- emprestimos so se aplicam a formatos emprestaveis definidos pela regra de negocio
- na v1, existe no maximo um `UserLibraryItem` ativo por `usuario + edicao`
- exclusao do item do acervo e logica; um item soft-deletado deixa de aparecer nas consultas e libera uma nova inclusao da mesma edicao
- registrar progresso em item da `Wishlist` deve ser rejeitado
- progresso final ou conclusao pode atualizar o item para `Completed`

## Filtros minimos esperados

- por status de leitura
- por shelf
- por favorito
- por formato
- por autor
- por titulo
- por data de atualizacao

Observacao da v1:

- filtro por tag entra quando o modulo de tags for materializado

## Ordenacao minima esperada

- titulo
- autor principal
- data de criacao
- data de atualizacao
- progresso
- data de conclusao

Observacao da v1:

- ordenacao por nota publicada depende do modulo de reviews e fica para fase posterior

## Busca no acervo

A busca interna deve considerar:

- titulo da obra
- titulo da edicao
- autores
- notas privadas quando apropriado

## Paginacao

Toda listagem do acervo deve ser paginada para suportar grande volume de itens.

## Historico de progresso

`ReadingProgressEntry` e recomendado para guardar:

- pagina ou percentual
- data do registro
- observacao opcional

Na v1, o registro de progresso tambem deve sincronizar `CurrentPage` e `ProgressPercent` no `UserLibraryItem`.

Isso permite mostrar leituras recentes e construir estatisticas futuras.

## API sugerida

- `GET /api/v1/library-items`
- `POST /api/v1/library-items`
- `GET /api/v1/library-items/{id}`
- `PUT /api/v1/library-items/{id}`
- `PATCH /api/v1/library-items/{id}/status`
- `POST /api/v1/library-items/{id}/progress`
- `DELETE /api/v1/library-items/{id}`

### Contratos operacionais recomendados na v1

`POST /api/v1/library-items`:

- recebe `BookEditionId`
- recebe `ShelfType`
- pode receber status inicial, favorito, notas privadas e metadados de posse
- nunca recebe `TenantId` ou `UserId` do cliente

`GET /api/v1/library-items`:

- usa `pageNumber` e `pageSize`
- aceita `sortBy` e `sortDirection`
- aceita filtros como `shelfType`, `readingStatus`, `isFavorite`, `acquisitionFormat`, `author`, `title` e `search`

`PATCH /api/v1/library-items/{id}/status`:

- altera apenas status e datas associadas a leitura
- deve aplicar regra de incremento de `ReadCount` quando houver nova conclusao

`POST /api/v1/library-items/{id}/progress`:

- cria `ReadingProgressEntry`
- atualiza snapshot de progresso do item
- pode alterar o status para `Reading`, `Rereading` ou `Completed` conforme o caso

## Observacao importante

O acervo nao substitui o catalogo global. Ele o referencia e adiciona comportamento especifico do usuario.
