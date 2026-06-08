# Acervo do Usuário

## Objetivo

Representar a experiência principal do produto: a relação do usuário com livros, formatos, leitura, organização e histórico.

## Entidade central

- `UserLibraryItem`

## Capacidades obrigatórias

- adicionar item ao acervo
- adicionar item à wishlist
- atualizar status de leitura
- registrar progresso
- favoritar
- informar formato
- informar localização física
- informar estado de conservação
- registrar quantidade de leituras
- armazenar notas privadas

## Campos de negócio recomendados

- `ShelfType`
- `ReadingStatus`
- `AcquisitionFormat`
- `IsFavorite`
- `CurrentPage`
- `ProgressPercent`
- `ReadCount`
- `StartedAt`
- `FinishedAt`
- `PhysicalLocation`
- `Condition`
- `PrivateNotes`

## Regras de domínio

- item em `Wishlist` pode não ter progresso
- item `Completed` deve ter `FinishedAt` ou progresso final compatível
- item `Reading` pode gerar múltiplos registros em `ReadingProgressEntry`
- `ReadCount` aumenta quando o status é concluído novamente
- empréstimos só se aplicam a formatos emprestáveis definidos pela regra de negócio

## Filtros mínimos esperados

- por status de leitura
- por shelf
- por favorito
- por formato
- por tag
- por autor
- por título
- por data de atualização

## Ordenação mínima esperada

- título
- autor principal
- data de criação
- data de atualização
- nota
- progresso
- data de conclusão

## Busca no acervo

A busca interna deve considerar:

- título da obra
- título da edição
- autores
- tags
- notas públicas ou privadas quando apropriado

## Paginação

Toda listagem do acervo deve ser paginada para suportar grande volume de itens.

## Histórico de progresso

`ReadingProgressEntry` é recomendado para guardar:

- página ou percentual
- data do registro
- observação opcional

Isso permite mostrar leituras recentes e construir estatísticas futuras.

## API sugerida

- `GET /api/v1/library-items`
- `POST /api/v1/library-items`
- `GET /api/v1/library-items/{id}`
- `PUT /api/v1/library-items/{id}`
- `PATCH /api/v1/library-items/{id}/status`
- `POST /api/v1/library-items/{id}/progress`
- `DELETE /api/v1/library-items/{id}`

## Observação importante

O acervo não substitui o catálogo global. Ele o referencia e adiciona comportamento específico do usuário.
