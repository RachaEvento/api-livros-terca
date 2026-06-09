# Providers de Livros

## Objetivo

Consumir fontes externas de livros sem acoplar a regra de negocio principal a contratos especificos de terceiros.

## Primeira integracao

- Open Library API

## Integracoes futuras previstas

- Google Books
- Amazon Books
- ISBNdb
- Goodreads APIs, se viavel

## Padrao arquitetural

Usar **Strategy Pattern + providers desacoplados**.

## Contratos sugeridos

### `IBookSearchProvider`

Responsavel por:

- buscar por ISBN
- buscar por titulo
- buscar por autor
- buscar por termo geral
- traduzir falhas externas em erro controlado para o orquestrador
- nunca devolver o contrato cru do provider para fora da infraestrutura

### `IBookSearchOrchestrator`

Responsavel por:

- executar providers em paralelo
- aplicar timeout por provider
- consolidar resultados
- remover duplicados
- ordenar por confianca
- degradar com seguranca quando uma fonte falhar

### `IBookResultNormalizer`

Responsavel por converter resposta externa em DTO interno padronizado.

## DTO interno sugerido

Exemplo de retorno normalizado:

```json
{
  "source": "open-library",
  "externalId": "OL12345M",
  "workExternalId": "OL67890W",
  "title": "Clean Architecture",
  "authors": ["Robert C. Martin"],
  "isbn10": "0134494164",
  "isbn13": "9780134494166",
  "publisher": "Prentice Hall",
  "publishedYear": 2017,
  "language": "en",
  "pageCount": 432,
  "coverImageUrl": "https://...",
  "confidenceScore": 0.94
}
```

Campos adicionais recomendados para o contrato interno:

- `workTitle`
- `description`
- `externalUrl`
- `publishedYear`
- `firstPublishedYear`
- `authors` como colecao de objetos ou nomes normalizados internamente

O contrato publico da API pode expor uma visao simplificada desse DTO, mas a fronteira entre `Application` e `Infrastructure` deve manter informacao suficiente para reconciliar `BookWork` e `BookEdition`.

## Fluxo de busca

1. API recebe consulta.
2. Servico monta `BookSearchQuery`.
3. Orquestrador executa providers habilitados.
4. Cada provider normaliza sua resposta.
5. Resultados sao deduplicados.
6. API devolve DTO unificado ao cliente.

## Regras de resiliencia

- falha de um provider nao derruba a busca inteira
- timeout por provider
- logs estruturados de erro por fonte
- resposta indica origem do resultado

## Regras de deduplicacao

- combinar por ISBN quando disponivel
- fallback para titulo + autor normalizados
- preservar melhor resultado como principal
- manter fontes de origem para rastreabilidade

Heuristica recomendada para chave de deduplicacao:

1. `isbn13`
2. `isbn10`
3. `source + externalId`
4. `normalizedTitle + normalizedAuthors + normalizedPublisher + language`

## Persistencia opcional

A busca externa nao precisa persistir tudo imediatamente. O sistema pode:

- retornar resultados sem salvar
- persistir somente quando o usuario adiciona um item ao acervo
- registrar referencias externas no momento da importacao

Na v1, a persistencia do catalogo deve acontecer preferencialmente durante `POST /api/v1/library-items`. O cliente pode reaproveitar o DTO normalizado retornado pela propria API de busca como payload aninhado do item de acervo, e a aplicacao deve reconciliar `BookWork` e `BookEdition` internamente antes de criar o `UserLibraryItem`. Em versoes futuras, cada provider pode enriquecer ou revalidar esse material de forma server-side antes de persistir.

## Endpoints recomendados da fase

### `GET /api/v1/books/search`

Parametros:

- `search`
- `isbn`
- `title`
- `author`
- `language`
- `pageNumber`
- `pageSize`

Regras:

- pelo menos um entre `search`, `isbn`, `title` ou `author` deve ser informado
- paginacao segue os padroes gerais da API
- a resposta sempre usa envelope padronizado
- cada item deve indicar `source`
- o endpoint e autenticado e deve usar o usuario atual resolvido via JWT para enriquecer o resultado
- cada item da busca pode incluir `existingLibraryItemId` e `existingShelfType` quando o livro ja estiver no acervo ativo do usuario autenticado
- o enriquecimento deve reutilizar a mesma logica de reconciliacao de edicao usada por `POST /api/v1/library-items`
- o enriquecimento deve funcionar tanto para resultados externos ainda nao persistidos quanto para resultados ja reconciliados com o catalogo interno
- a implementacao deve resolver correspondencias em lote e consultar o acervo do usuario sem N+1 queries

Exemplo de item enriquecido:

```json
{
  "source": "open-library",
  "externalId": "OL12345M",
  "workExternalId": "OL67890W",
  "title": "Clean Architecture",
  "authors": ["Robert C. Martin"],
  "isbn13": "9780134494166",
  "coverImageUrl": "https://...",
  "confidenceScore": 0.98,
  "existingLibraryItemId": "9b5b5e2d-0000-0000-0000-000000000000",
  "existingShelfType": "Collection"
}
```

### Persistencia via `POST /api/v1/library-items`

Payload sugerido para o objeto `book` aninhado:

```json
{
  "source": "open-library",
  "externalId": "OL12345M",
  "workExternalId": "OL67890W",
  "title": "Clean Architecture",
  "workTitle": "Clean Architecture",
  "authors": ["Robert C. Martin"],
  "isbn10": "0134494164",
  "isbn13": "9780134494166",
  "publisher": "Prentice Hall",
  "publishedYear": 2017,
  "firstPublishedYear": 2017,
  "language": "en",
  "pageCount": 432,
  "coverImageUrl": "https://...",
  "description": "..."
}
```

Regras:

- o payload deve usar contrato normalizado interno, nunca o schema cru do provider
- `POST /api/v1/library-items` deve aceitar `BookEditionId` existente ou um objeto `book` com metadados normalizados da busca
- quando o objeto `book` for informado, a aplicacao deve reconciliar `BookWork`, `BookEdition`, `Author`, `Publisher` e `ExternalBookReference` antes de criar o item do usuario
- quando ja existir uma edicao compativel, o comportamento esperado e reconciliar e enriquecer, nao duplicar
- ISBN continua sendo o principal criterio de deduplicacao, com fallback para campos normalizados
- nao deve existir um endpoint publico separado de importacao na v1; importar passa a ser detalhe interno do caso de uso de adicionar ao acervo

## Decisao importante

O restante do sistema nunca deve depender diretamente do formato retornado pelo provider. Toda integracao externa deve ser traduzida para contratos internos.
