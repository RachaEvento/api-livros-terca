# Padrões de API

## Princípios

- REST consistente
- DTOs explícitos
- paginação obrigatória em listagens
- filtros previsíveis
- respostas padronizadas

## Base route

```text
/api/v1
```

## Convenção de endpoints

- recursos no plural
- ações especiais como sub-recursos quando fizer sentido
- evitar verbos no path, exceto operações de domínio específicas

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

Mesmo em endpoints técnicos e de bootstrap, o envelope deve permanecer consistente. Apenas respostas `204 No Content` podem ser retornadas sem corpo quando isso fizer mais sentido semântico.

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

## Parâmetros de paginação

- `pageNumber`
- `pageSize`

Limites sugeridos:

- padrão `pageSize = 20`
- máximo `pageSize = 100`

## Parâmetros de ordenação

- `sortBy`
- `sortDirection`

Valores aceitos para direção:

- `asc`
- `desc`

## Filtros

Filtros devem ser específicos por recurso, por exemplo:

- `status`
- `shelf`
- `favorite`
- `tagId`
- `author`
- `search`

## Versionamento

Usar prefixo `/v1` na rota. Mudanças breaking futuras devem resultar em nova versão.

## Swagger/OpenAPI

Obrigatório documentar:

- payloads de entrada
- respostas de sucesso
- respostas de erro
- autenticação Bearer
- exemplos de uso

Na fundação da solução, Swagger também deve deixar visível:

- endpoint de health/readiness
- endpoint técnico inicial de verificação da API
- esquema de erro padronizado
- versionamento base em `/api/v1`

## Idempotência

Operações de atualização devem ser idempotentes quando possível. Evitar efeitos colaterais ocultos em `GET`.

## Observação

Mesmo com envelope padronizado, o conteúdo de erro deve seguir o padrão definido em `17-exception-handling.md`.
