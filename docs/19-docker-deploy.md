# Docker e Deploy

## Objetivo

Padronizar execução local e preparar caminho de deploy reproduzível.

## Stack mínima

- API ASP.NET Core
- PostgreSQL

Opcional em ambiente local:

- pgAdmin

## Arquivos esperados

- `Dockerfile`
- `docker-compose.yml`
- `.env.example`
- `.dockerignore`

## Variáveis de ambiente mínimas

- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS`
- `DATABASE_URL`
- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__AccessTokenMinutes`
- `Jwt__RefreshTokenDays`
- `BookProviders__OpenLibrary__Enabled`
- `BookProviders__OpenLibrary__BaseUrl`
- `BookProviders__OpenLibrary__UserAgent`
- `BookProviders__OpenLibrary__ContactEmail`
- `BookProviders__OpenLibrary__TimeoutSeconds`
- `POSTGRES_DB`
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_PORT`

Quando `DATABASE_URL` estiver presente, ele deve ter precedência sobre `ConnectionStrings__DefaultConnection`.

Formato suportado:

- `postgresql://usuario:senha@host:porta/database?schema=public`

Para a Open Library, recomenda-se informar um `User-Agent` identificável e um contato operacional, em linha com a documentação pública do provider.

## Estratégia de banco

Usar EF Core Migrations.

Recomendações:

- migrations revisadas em código
- aplicar migration na pipeline ou no startup controlado
- não depender de criação manual do schema

## Fluxo local esperado

1. subir PostgreSQL com Docker
2. subir a API com as variáveis de ambiente corretas
3. aplicar migrations por CLI ou startup controlado
4. acessar Swagger

## Convenções operacionais da fundação

- a API deve expor porta HTTP configurável por variável de ambiente
- o container final deve executar apenas o artefato publicado
- o `docker-compose` deve declarar dependência do PostgreSQL com `healthcheck`
- a string de conexão usada pela API deve apontar para o serviço `postgres` quando executada via compose

## Exemplo de serviços no compose

- `api`
- `postgres`

## Requisitos do container da API

- build multi-stage
- imagem final enxuta
- portas configuráveis
- leitura de variáveis de ambiente

## Requisitos do PostgreSQL

- volume persistente
- usuário, senha e database configuráveis
- healthcheck quando possível

## Caminho futuro de deploy

O projeto deve poder ser publicado em:

- VPS
- container host
- orquestradores como Kubernetes no futuro
- plataformas de container compatíveis

## Swagger e documentação operacional

Ao subir localmente, a API deve expor Swagger para facilitar integração com frontend e testes manuais.
