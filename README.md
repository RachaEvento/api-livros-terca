# Meu Acervo API
Equipe: Miguel, Amanda, Diogo, Débora
Backend em ASP.NET Core para o sistema **Meu Acervo**, uma plataforma de gerenciamento de biblioteca pessoal com catálogo bibliográfico normalizado, acervo multi-tenant e integração inicial com a Open Library.

O projeto segue **Spec-Driven Development com apoio de IA**: a documentação em [docs](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs) é a referência principal para comportamento, contratos e decisões arquiteturais.

## Visão do produto

O objetivo da API é permitir que cada usuário:

- crie conta e autentique-se com JWT
- mantenha um acervo pessoal isolado por tenant
- pesquise livros em fontes externas
- registre obras, edições e itens concretos do acervo
- acompanhe status e progresso de leitura
- favorite livros, publique reviews e controle empréstimos
- personalize metadados com campos customizados
- exponha parte do perfil publicamente

O domínio separa explicitamente:

- `BookWork`: obra intelectual
- `BookEdition`: edição específica
- `UserLibraryItem`: item do acervo do usuário

## Princípios do projeto

- `Tenant first`: todo dado do usuário respeita isolamento por tenant
- controllers magros, sem regra de negócio
- DTOs em toda a API pública
- validação explícita com FluentValidation
- catálogo global separado do acervo tenant-scoped
- auditoria básica nas entidades relevantes
- arquitetura preparada para novos providers, cache, mensageria e evolução social

## Stack

- .NET SDK 10.0.201
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Bearer Authentication
- Swagger / OpenAPI
- FluentValidation
- AutoMapper
- Docker / Docker Compose

## Arquitetura da solução

```text
src/
  MeuAcervo.API/             # Controllers, auth, pipeline HTTP, Swagger
  MeuAcervo.Application/     # Casos de uso, DTOs, serviços, validators, mappings
  MeuAcervo.Domain/          # Entidades, enums, contratos e regras centrais
  MeuAcervo.Infrastructure/  # EF Core, repositórios, providers e serviços técnicos
  MeuAcervo.Shared/          # Contratos compartilhados, paginação, envelope de resposta
docs/                        # Especificação funcional e arquitetural
```

## Módulos já cobertos

- autenticação e sessão JWT
- catálogo bibliográfico
- busca externa de livros via Open Library
- acervo do usuário
- reviews
- empréstimos
- campos customizados
- perfil público do leitor
- endpoints de sistema e health check

## Endpoints principais

Base route: `/api/v1`

### Sistema

- `GET /api/v1/system/info`
- `GET /api/v1/system/health`

### Autenticação

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/logout`
- `GET /api/v1/auth/me`

### Livros e catálogo

- `GET /api/v1/books/search`

### Acervo

- `GET /api/v1/library-items`
- `POST /api/v1/library-items`
- `GET /api/v1/library-items/{id}`
- `PUT /api/v1/library-items/{id}`
- `PATCH /api/v1/library-items/{id}/status`
- `POST /api/v1/library-items/{id}/progress`
- `DELETE /api/v1/library-items/{id}`

### Reviews, empréstimos e campos customizados

- `GET|POST|PUT|DELETE /api/v1/library-items/{libraryItemId}/review`
- `POST /api/v1/library-items/{libraryItemId}/loans`
- `PATCH /api/v1/library-items/{libraryItemId}/loans/{loanId}/return`
- `PUT /api/v1/library-items/{libraryItemId}/custom-fields`
- `GET|POST /api/v1/custom-fields`
- `PUT|DELETE /api/v1/custom-fields/{id}`

### Perfil

- `GET /api/v1/profile`
- `PUT /api/v1/profile`
- `GET /api/v1/users/{username}`
- `GET /api/v1/users/{username}/library`
- `GET /api/v1/users/{username}/favorites`
- `GET /api/v1/users/{username}/reviews`
- `GET /api/v1/users/{username}/activity`

## Padrões de API

- rotas versionadas em `/api/v1`
- respostas envelopadas em `ApiResponse`
- paginação padronizada com `pageNumber` e `pageSize`
- ordenação por `sortBy` e `sortDirection`
- enums serializados como string
- autenticação Bearer via JWT

Exemplo de resposta de sucesso:

```json
{
  "success": true,
  "data": {},
  "meta": {
    "traceId": "..."
  }
}
```

## Multi-tenancy

- cada usuário nasce com um tenant pessoal
- o contexto de tenant é resolvido a partir do JWT
- dados do catálogo bibliográfico são globais
- dados de acervo, reviews, empréstimos, perfil e customização são tenant-scoped
- a API nunca deve confiar em `TenantId` enviado livremente pelo cliente

## Como executar localmente

### Pré-requisitos

- .NET SDK 10
- Docker e Docker Compose

### Opção 1: Docker Compose

1. Crie o arquivo `.env` a partir de [.env.example](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/.env.example).
2. Defina uma `Jwt__Key` forte com pelo menos 32 caracteres.
3. Execute:

```powershell
docker compose up --build
```

4. Acesse:

- Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)
- API: [http://localhost:8080](http://localhost:8080)

### Opção 2: `dotnet run`

1. Suba apenas o PostgreSQL, por exemplo com Docker:

```powershell
docker compose up postgres -d
```

2. Configure as variáveis de ambiente necessárias, em especial:

- `DATABASE_URL` ou `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Database__ApplyMigrationsOnStartup`

3. Execute a API:

```powershell
dotnet run --project src/MeuAcervo.API
```

4. Abra o Swagger no endereço informado pelo ASP.NET Core.

## Configuração

As variáveis mínimas esperadas pelo projeto incluem:

- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS`
- `DATABASE_URL`
- `ConnectionStrings__DefaultConnection`
- `Database__ApplyMigrationsOnStartup`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__AccessTokenMinutes`
- `BookProviders__OpenLibrary__Enabled`
- `BookProviders__OpenLibrary__BaseUrl`
- `BookProviders__OpenLibrary__UserAgent`
- `BookProviders__OpenLibrary__ContactEmail`
- `BookProviders__OpenLibrary__TimeoutSeconds`

Quando `DATABASE_URL` estiver presente, ele tem precedência sobre `ConnectionStrings__DefaultConnection`.

## Banco de dados e migrations

- a API aplica migrations pendentes no startup por padrão
- o comportamento pode ser desligado com `Database__ApplyMigrationsOnStartup=false`
- o projeto de migrations fica em [src/MeuAcervo.Infrastructure/Data/Migrations](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/src/MeuAcervo.Infrastructure/Data/Migrations)

Para aplicar migrations manualmente:

```powershell
dotnet ef database update --project src/MeuAcervo.Infrastructure --startup-project src/MeuAcervo.API
```

## Documentação como fonte de verdade

Antes de alterar código, leia nesta ordem:

1. [docs/01-visao-geral.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/01-visao-geral.md)
2. [docs/02-arquitetura.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/02-arquitetura.md)
3. [docs/03-convencoes.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/03-convencoes.md)
4. [docs/04-entidades.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/04-entidades.md)
5. [docs/05-multi-tenancy.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/05-multi-tenancy.md)
6. os documentos específicos do módulo afetado

Se houver divergência entre código e documentação, trate a documentação como fonte de verdade temporária até a divergência ser resolvida explicitamente.

## Fluxo de contribuição

Toda mudança neste repositório deve seguir o fluxo abaixo:

1. entender a necessidade de negócio
2. identificar os documentos impactados
3. atualizar ou confirmar a especificação
4. avaliar impacto em domínio, aplicação, infraestrutura e API
5. implementar por camadas
6. validar compilação, DI, tenancy, autorização e contratos
7. fechar a tarefa com documentação atualizada

## Observações importantes

- o arquivo `appsettings.json` pode servir para desenvolvimento local, mas segredos não devem permanecer hardcoded em ambientes reais
- a Open Library deve ser acessada com `User-Agent` identificável e contato operacional
- o projeto já está estruturado para crescer com novos providers, cache e jobs assíncronos

## Referências

- visão geral: [docs/01-visao-geral.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/01-visao-geral.md)
- arquitetura: [docs/02-arquitetura.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/02-arquitetura.md)
- autenticação: [docs/07-autenticacao-jwt.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/07-autenticacao-jwt.md)
- padrões de API: [docs/16-api-patterns.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/16-api-patterns.md)
- docker e deploy: [docs/19-docker-deploy.md](/C:/Users/migue/Desktop/DIVISAO-TERCA/api-livros-terca/docs/19-docker-deploy.md)
