# Convenções

## Convenções gerais

- Código em C# moderno e idiomático.
- Métodos assíncronos com sufixo `Async`.
- Datas sempre em UTC na persistência.
- IDs preferencialmente como `Guid`.
- Enums serializados como string na API.
- `CancellationToken` em operações de I/O sempre que viável.

## Convenções de nomenclatura

- Classes: `PascalCase`
- Interfaces: prefixo `I`
- Métodos e propriedades: `PascalCase`
- Parâmetros e variáveis locais: `camelCase`
- Campos privados: `_camelCase`
- Rotas REST em `kebab-case` apenas se necessário; preferir nomes simples e consistentes

## Convenções de pasta

- `Controllers`: apenas endpoints e adaptação HTTP
- `Services` ou `Features`: regras de aplicação
- `Repositories`: acesso a dados
- `Validators`: validação de DTOs
- `Mappings`: profiles do AutoMapper
- `Providers`: integrações externas

## Convenções de API

- Base route: `/api/v1`
- Resposta de sucesso deve ser consistente
- Erros devem usar um formato padronizado
- Paginação, filtros e ordenação devem seguir contrato uniforme

## Convenções de entidades

- Toda entidade relevante deve possuir auditoria básica.
- Entidades tenant-scoped devem possuir `TenantId`.
- Soft delete com `DeletedAtUtc` e `IsDeleted` quando necessário.
- Não expor entidades diretamente em controllers.

## Convenções de validação

- Validação estrutural em FluentValidation.
- Regras de domínio complexas em serviços ou entidades.
- Mensagens de erro claras e determinísticas.

## Convenções de persistência

- Configuração de EF Core via `IEntityTypeConfiguration<T>`.
- Índices definidos explicitamente.
- Constraints de unicidade para evitar duplicidade funcional.
- Migrations versionadas e revisáveis.

## Convenções para spec-driven development

Antes de implementar:

1. localizar os documentos impactados
2. revisar a especificação
3. ajustar a documentação se o requisito evoluiu
4. só então alterar código

Ao finalizar:

1. validar compilação
2. revisar contratos da API
3. atualizar documentação relevante

## Convenções de versionamento de contrato

- Alterações breaking devem ser evitadas na v1.
- Se inevitáveis, registrar em documentação e versionar endpoint.
- Campos opcionais novos são preferíveis a mudanças incompatíveis.

## Convenções para queries

- Toda query tenant-scoped deve filtrar por `TenantId`.
- Paginação padrão por `pageNumber` e `pageSize`.
- Ordenação por `sortBy` e `sortDirection`.
- Filtros devem ser explicitamente nomeados, evitando parâmetros ambíguos.

## Convenções de segurança

- Nunca confiar em `TenantId` vindo do body quando o dado pertence ao usuário autenticado.
- Claims do JWT são a fonte primária para contexto do tenant.
- Access token deve expirar em janela curta e ser tratado com cuidado pelo cliente.
- Segredos só via variáveis de ambiente ou secret store.

## Convenções de documentação

- Cada módulo relevante deve ter documento dedicado em `docs/`.
- Mudanças de comportamento exigem atualização do documento correspondente.
- Decisões importantes devem ser descritas de forma objetiva e reutilizável.
