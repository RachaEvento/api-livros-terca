# Arquitetura

## Estilo arquitetural

O sistema deve usar **arquitetura em camadas com fronteiras limpas**, inspirada em Clean Architecture, sem exigir complexidade desnecessária. A meta é separar responsabilidades para permitir evolução segura.

## Camadas propostas

### API

Responsável por:

- controllers HTTP
- configuração do pipeline
- autenticação e autorização
- middleware global
- Swagger/OpenAPI
- serialização e contrato público

### Application

Responsável por:

- casos de uso
- serviços de aplicação
- DTOs
- validators
- mapeamentos AutoMapper
- contratos de integração
- regras de orquestração

### Domain

Responsável por:

- entidades
- value objects
- enums
- regras de negócio centrais
- interfaces de domínio quando aplicável

### Infrastructure

Responsável por:

- EF Core
- repositórios
- DbContext
- providers externos
- servicos tecnicos de emissao e validacao de JWT
- implementação de serviços técnicos

## Estrutura sugerida

```text
src/
  MeuAcervo.API/
    Controllers/
    Middlewares/
    Auth/
    Configurations/
    Common/
  MeuAcervo.Application/
    DTOs/
    Services/
    Validators/
    Mappings/
    Interfaces/
    Features/
  MeuAcervo.Domain/
    Entities/
    Enums/
    ValueObjects/
    Exceptions/
  MeuAcervo.Infrastructure/
    Data/
    Repositories/
    Providers/
    Auth/
    Migrations/
    Configurations/
  MeuAcervo.Shared/
    Contracts/
    Pagination/
    Results/
```

## Referências entre projetos

Para preservar fronteiras limpas, as referências devem seguir esta direção:

- `MeuAcervo.API` -> `MeuAcervo.Application`, `MeuAcervo.Infrastructure`, `MeuAcervo.Shared`
- `MeuAcervo.Application` -> `MeuAcervo.Domain`, `MeuAcervo.Shared`
- `MeuAcervo.Infrastructure` -> `MeuAcervo.Application`, `MeuAcervo.Domain`, `MeuAcervo.Shared`
- `MeuAcervo.Domain` -> sem dependência de infraestrutura
- `MeuAcervo.Shared` -> sem dependência de regras de negócio

`Infrastructure` pode implementar contratos definidos em `Application` ou `Domain`, mas as camadas internas não podem depender de detalhes técnicos da camada externa.

## Módulos de negócio

Os módulos lógicos da solução são:

- Identidade e acesso
- Catálogo bibliográfico
- Busca externa de livros
- Acervo do usuário
- Reviews e notas
- Empréstimos
- Perfil social público
- Campos customizados

## Fluxo principal de requisição

1. Requisição chega no controller.
2. JWT é validado e o contexto do tenant é resolvido.
3. DTO de entrada é validado.
4. Serviço de aplicação executa o caso de uso.
5. Repositórios e providers externos são acionados quando necessário.
6. Resultado é convertido em DTO de saída.
7. API retorna resposta padronizada.

## Regras de separação

- Controller não decide regra de domínio.
- Repositório não contém regra de negócio complexa.
- Provider externo não vaza contrato externo para o restante do sistema.
- Entidade não conhece detalhes de transporte HTTP.
- DTO não substitui entidade de domínio.

## Estratégia para livros

Separar o domínio em três níveis:

- `BookWork`: obra abstrata
- `BookEdition`: edição específica
- `UserLibraryItem`: item concreto do acervo do usuário

Isso reduz duplicidade, melhora a normalização e prepara o sistema para múltiplos provedores de livros.

## Integrações externas

O sistema deve adotar interfaces desacopladas para providers:

- `IBookSearchProvider`
- `IBookMetadataNormalizer`
- `IBookProviderOrchestrator`

Cada provider converte respostas externas em DTOs internos padronizados.

## Padrões arquiteturais obrigatórios

- Repository Pattern
- Service Layer
- DTOs
- FluentValidation
- AutoMapper
- Middleware global de exceções
- Paginação padronizada
- Soft delete quando aplicável
- Auditoria básica

## Fundação obrigatória da solução

A primeira entrega funcional da base deve incluir:

- composition root por camada com extensões de DI
- `DbContext` central da aplicação
- abstrações de auditoria e tenancy
- middleware global de exceções
- envelope padronizado de respostas
- configuração de autenticação JWT pronta para uso
- Swagger/OpenAPI com esquema Bearer
- readiness para migrations e execução em Docker

## Decisões arquiteturais assumidas

- O catálogo bibliográfico pode ser compartilhado globalmente.
- Dados de leitura, review, empréstimos e customizações são tenant-scoped.
- Cada usuário começa com um tenant pessoal.
- A arquitetura já considera expansão futura para tenants de grupo.

## Preparação para evolução

Mesmo sem implementar tudo na v1, a arquitetura deve deixar espaço para:

- cache distribuído
- mensageria/eventos
- jobs assíncronos
- novos providers de livros
- recursos sociais avançados
- possível decomposição em microsserviços no futuro
