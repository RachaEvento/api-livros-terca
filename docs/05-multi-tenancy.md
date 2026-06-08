# Multi-Tenancy

## Objetivo

Garantir isolamento seguro entre usuários sem impedir evolução futura para tenants compartilhados por família, clube de leitura ou organização.

## Estratégia adotada

- Cada usuário nasce com um **tenant pessoal**.
- Os dados operacionais do acervo são sempre vinculados a `TenantId`.
- O catálogo bibliográfico pode ser global e compartilhado.
- O JWT carrega o contexto de tenant.

## Tipos de dados

### Dados globais

- `Permission`
- `Author`
- `Publisher`
- `BookWork`
- `BookEdition`
- `BookEditionAuthor`
- `ExternalBookReference`

### Dados tenant-scoped

- `Tenant`
- `User`
- `Role`
- `UserRole`
- `RefreshToken`
- `UserLibraryItem`
- `ReadingProgressEntry`
- `Tag`
- `Review`
- `Loan`
- `CustomFieldDefinition`
- `CustomFieldOption`
- `CustomFieldValue`
- `UserProfile`

## Resolução de tenant

O tenant deve ser resolvido a partir do JWT autenticado, por meio de claims como:

- `sub`
- `tid`
- `username`
- `roles`

Nunca confiar em `TenantId` enviado pelo cliente quando o recurso pertence ao usuário autenticado.

## Aplicação de isolamento

### No domínio e aplicação

- Serviços devem exigir o tenant atual para operações tenant-scoped.
- Regras de acesso devem sempre comparar `TenantId` do recurso com o contexto autenticado.

### Na persistência

- Entidades tenant-scoped devem implementar um contrato como `ITenantScoped`.
- Repositórios devem filtrar por `TenantId`.
- Pode ser usado `HasQueryFilter` no EF Core para reforço, sem depender apenas disso.
- Em autenticação, login e refresh podem consultar dados globais necessários para localizar o usuário, mas a sessão emitida deve sempre carregar e reafirmar o `TenantId` correto.

### Na API

- Controllers não recebem `TenantId` livremente para criação de recursos do usuário.
- O contexto do tenant é obtido de um `CurrentUserContext`.

## Expansão futura

O modelo deve permitir que, no futuro:

- um usuário participe de múltiplos tenants
- um tenant tenha múltiplos usuários
- permissões sejam atribuídas por tenant

Por isso, mesmo na v1, vale manter os conceitos separados e evitar acoplamento rígido entre `User` e `Tenant`.

## Regras de segurança

- Toda query tenant-scoped deve filtrar por `TenantId`.
- Toda escrita deve validar se o recurso pai pertence ao tenant.
- Logs não devem expor dados sensíveis de outros tenants.
- Endpoints públicos jamais podem retornar dados privados por falha de filtro.
- Policies e verificações de permissão devem considerar o tenant da sessão autenticada, não parâmetros livres do cliente.

## Checklist de revisão

Ao implementar qualquer feature tenant-scoped, confirmar:

- a entidade possui `TenantId`
- o repositório filtra por tenant
- o serviço valida posse do recurso
- o endpoint usa contexto autenticado
- a resposta não mistura dados de outro tenant
