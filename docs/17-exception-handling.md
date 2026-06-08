# Tratamento de Exceções

## Objetivo

Centralizar erros, evitar respostas inconsistentes e fornecer informações úteis para cliente e observabilidade.

## Estratégia

- middleware global de exceções
- tradução de exceções conhecidas para status HTTP corretos
- logs estruturados
- `traceId` em toda resposta de erro

## Exceções sugeridas

- `ValidationException`
- `NotFoundException`
- `ConflictException`
- `UnauthorizedException`
- `ForbiddenException`
- `ExternalProviderException`
- `BusinessRuleException`

## Mapeamento HTTP sugerido

| Exceção | Status |
| --- | --- |
| `ValidationException` | `400 Bad Request` |
| `UnauthorizedException` | `401 Unauthorized` |
| `ForbiddenException` | `403 Forbidden` |
| `NotFoundException` | `404 Not Found` |
| `ConflictException` | `409 Conflict` |
| `BusinessRuleException` | `422 Unprocessable Entity` |
| `ExternalProviderException` | `502 Bad Gateway` ou `503 Service Unavailable` |
| erro inesperado | `500 Internal Server Error` |

## Resposta de erro sugerida

```json
{
  "success": false,
  "error": {
    "code": "validation_error",
    "message": "One or more validation errors occurred.",
    "details": {
      "email": ["Email is required."]
    }
  },
  "meta": {
    "traceId": "..."
  }
}
```

Campos esperados no objeto `error`:

- `code`: identificador estável e legível por cliente
- `message`: mensagem principal para consumo da API
- `details`: dicionário opcional para erros de validação ou contexto adicional

## Diretrizes

- não expor stack trace em produção
- mensagens de domínio devem ser compreensíveis
- falhas externas devem preservar contexto técnico apenas em log
- violações de tenant podem resultar em `404` ou `403` conforme estratégia adotada

## Logging

Registrar:

- categoria do erro
- usuário autenticado quando houver
- tenant
- endpoint
- provider externo, se aplicável
- `traceId`

## Integração com validação

Erros de FluentValidation devem ser agregados e padronizados pela mesma estrutura de resposta.

## Fundação mínima obrigatória

Mesmo antes dos módulos de negócio, a solução já deve sair com:

- middleware único para tradução de exceções
- mapeamento de exceções de domínio e aplicação
- `traceId` propagado a partir do contexto HTTP
- logging estruturado por falha
