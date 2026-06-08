# Padroes de API

## Principios

- REST consistente
- DTOs explicitos
- paginacao obrigatoria em listagens
- filtros previsiveis
- respostas padronizadas

## Base route

```text
/api/v1
```

## Convencao de endpoints

- recursos no plural
- acoes especiais como sub-recursos quando fizer sentido
- evitar verbos no path, exceto operacoes de dominio especificas

## Resposta de sucesso sugerida

```json
{
  "success": true,
  "data": {},
  "meta": {
    "traceId": "..."
  }
}
```

Mesmo em endpoints tecnicos e de bootstrap, o envelope deve permanecer consistente. Apenas respostas `204 No Content` podem ser retornadas sem corpo quando isso fizer mais sentido semantico.

## Resposta paginada sugerida

```json
{
  "success": true,
  "data": [],
  "meta": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 125,
    "totalPages": 7,
    "traceId": "..."
  }
}
```

## Parametros de paginacao

- `pageNumber`
- `pageSize`

Limites sugeridos:

- padrao `pageSize = 20`
- maximo `pageSize = 100`

## Parametros de ordenacao

- `sortBy`
- `sortDirection`

Valores aceitos para direcao:

- `asc`
- `desc`

## Filtros

Filtros devem ser especificos por recurso, por exemplo:

- `status`
- `shelf`
- `favorite`
- `tagId`
- `author`
- `search`

No modulo de acervo da v1, o recurso `library-items` deve aceitar ao menos:

- `shelfType`
- `readingStatus`
- `isFavorite`
- `acquisitionFormat`
- `author`
- `title`
- `search`
- `updatedFrom`
- `updatedTo`

## Versionamento

Usar prefixo `/v1` na rota. Mudancas breaking futuras devem resultar em nova versao.

## Swagger/OpenAPI

Obrigatorio documentar:

- payloads de entrada
- respostas de sucesso
- respostas de erro
- autenticacao Bearer
- exemplos de uso

Na fundacao da solucao, Swagger tambem deve deixar visivel:

- endpoint de health/readiness
- endpoint tecnico inicial de verificacao da API
- esquema de erro padronizado
- versionamento base em `/api/v1`

## Idempotencia

Operacoes de atualizacao devem ser idempotentes quando possivel. Evitar efeitos colaterais ocultos em `GET`.

## Observacao

Mesmo com envelope padronizado, o conteudo de erro deve seguir o padrao definido em `17-exception-handling.md`.
