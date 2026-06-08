# Empréstimos

## Objetivo

Controlar livros emprestados a terceiros sem transformar o sistema em um módulo completo de logística.

## Escopo da v1

- registrar empréstimo
- consultar empréstimos ativos e históricos
- registrar devolução
- opcionalmente marcar atraso

## Entidade sugerida

### Loan

Campos:

- `Id`
- `TenantId`
- `UserLibraryItemId`
- `BorrowerName`
- `BorrowerContact`
- `LoanedAtUtc`
- `DueAtUtc`
- `ReturnedAtUtc`
- `Status`
- `Notes`
- `CreatedAtUtc`
- `UpdatedAtUtc`

## Status sugeridos

- `Active`
- `Returned`
- `Overdue`
- `Lost`
- `Cancelled`

## Regras de domínio

- um item não pode ter mais de um empréstimo ativo ao mesmo tempo
- somente itens emprestáveis podem ser emprestados
- registrar devolução encerra o empréstimo
- status `Overdue` pode ser calculado a partir da data prevista

## Regras de elegibilidade

Por padrão, permitir empréstimo apenas para itens:

- do acervo, não da wishlist
- não excluídos
- não já emprestados
- compatíveis com empréstimo físico ou regra equivalente

## API sugerida

- `GET /api/v1/loans`
- `POST /api/v1/library-items/{id}/loans`
- `PATCH /api/v1/loans/{id}/return`
- `GET /api/v1/loans/{id}`

## Evolução futura

- lembretes automáticos
- notificações
- histórico analítico de atrasos
