# Providers de Livros

## Objetivo

Consumir fontes externas de livros sem acoplar a regra de negócio principal a contratos específicos de terceiros.

## Primeira integração

- Open Library API

## Integrações futuras previstas

- Google Books
- Amazon Books
- ISBNdb
- Goodreads APIs, se viável

## Padrão arquitetural

Usar **Strategy Pattern + providers desacoplados**.

## Contratos sugeridos

### `IBookSearchProvider`

Responsável por:

- buscar por ISBN
- buscar por título
- buscar por autor
- buscar por termo geral
- traduzir falhas externas em erro controlado para o orquestrador
- nunca devolver o contrato cru do provider para fora da infraestrutura

### `IBookSearchOrchestrator`

Responsável por:

- executar providers em paralelo
- aplicar timeout por provider
- consolidar resultados
- remover duplicados
- ordenar por confiança
- degradar com segurança quando uma fonte falhar

### `IBookResultNormalizer`

Responsável por converter resposta externa em DTO interno padronizado.

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
- `authors` como coleção de objetos ou nomes normalizados internamente

O contrato público da API pode expor uma visão simplificada desse DTO, mas a fronteira entre `Application` e `Infrastructure` deve manter informação suficiente para reconciliar `BookWork` e `BookEdition`.

## Fluxo de busca

1. API recebe consulta.
2. Serviço monta `BookSearchQuery`.
3. Orquestrador executa providers habilitados.
4. Cada provider normaliza sua resposta.
5. Resultados são deduplicados.
6. API devolve DTO unificado ao cliente.

## Regras de resiliência

- falha de um provider não derruba a busca inteira
- timeout por provider
- logs estruturados de erro por fonte
- resposta indica origem do resultado

## Regras de deduplicação

- combinar por ISBN quando disponível
- fallback para título + autor normalizados
- preservar melhor resultado como principal
- manter fontes de origem para rastreabilidade

Heurística recomendada para chave de deduplicação:

1. `isbn13`
2. `isbn10`
3. `source + externalId`
4. `normalizedTitle + normalizedAuthors + normalizedPublisher + language`

## Persistência opcional

A busca externa não precisa persistir tudo imediatamente. O sistema pode:

- retornar resultados sem salvar
- persistir somente quando o usuário adiciona um item ao acervo
- registrar referências externas no momento da importação

Na v1, a importação pode receber um DTO normalizado retornado pela própria API de busca e usar esse payload para reconciliar o catálogo interno. Em versões futuras, cada provider pode enriquecer ou revalidar esse material de forma server-side antes de persistir.

## Endpoints recomendados da fase

### `GET /api/v1/books/search`

Parâmetros:

- `search`
- `isbn`
- `title`
- `author`
- `language`
- `pageNumber`
- `pageSize`

Regras:

- pelo menos um entre `search`, `isbn`, `title` ou `author` deve ser informado
- paginação segue os padrões gerais da API
- a resposta sempre usa envelope padronizado
- cada item deve indicar `source`

### `POST /api/v1/books/import`

Payload sugerido:

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

- o payload de importação deve usar contrato normalizado interno, nunca o schema cru do provider
- a importação deve reconciliar `BookWork`, `BookEdition`, `Author`, `Publisher` e `ExternalBookReference`
- quando já existir uma edição compatível, o comportamento esperado é reconciliar e enriquecer, não duplicar
- ISBN continua sendo o principal critério de deduplicação, com fallback para campos normalizados

## Decisão importante

O restante do sistema nunca deve depender diretamente do formato retornado pelo provider. Toda integração externa deve ser traduzida para contratos internos.
