# Emprestimos

## Objetivo

Controlar livros emprestados a terceiros sem transformar o sistema em um modulo completo de logistica.

## Escopo da v1

- registrar emprestimo
- consultar emprestimos ativos e historicos
- registrar devolucao
- opcionalmente marcar atraso por calculo

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

Na v1, `Overdue` pode ser tratado como status efetivo calculado a partir de `DueAtUtc` para emprestimos ainda ativos.

## Regras de dominio

- um item nao pode ter mais de um emprestimo ativo ao mesmo tempo
- somente itens elegiveis podem ser emprestados
- registrar devolucao encerra o emprestimo
- status `Overdue` pode ser calculado a partir da data prevista
- emprestimo e item devem pertencer ao mesmo tenant

## Regras de elegibilidade

Por padrao, permitir emprestimo apenas para itens:

- do acervo, nao da wishlist
- nao excluidos
- nao ja emprestados
- com `AcquisitionFormat = Physical`

## API sugerida

- `GET /api/v1/loans`
- `GET /api/v1/loans/{id}`
- `POST /api/v1/library-items/{id}/loans`
- `PATCH /api/v1/loans/{id}/return`

## Evolucao futura

- lembretes automaticos
- notificacoes
- historico analitico de atrasos
