# Docker e Deploy

## Objetivo

Padronizar execucao local e preparar caminho de deploy reproduzivel.

## Stack minima

- API ASP.NET Core
- PostgreSQL

Opcional em ambiente local:

- pgAdmin

## Arquivos esperados

- `Dockerfile`
- `docker-compose.yml`
- `.env.example`
- `.dockerignore`

## Variaveis de ambiente minimas

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
- `POSTGRES_DB`
- `POSTGRES_USER`
- `POSTGRES_PASSWORD`
- `POSTGRES_PORT`

Quando `DATABASE_URL` estiver presente, ele deve ter precedencia sobre `ConnectionStrings__DefaultConnection`.

Formato suportado:

- `postgresql://usuario:senha@host:porta/database?schema=public`

Para a Open Library, recomenda-se informar um `User-Agent` identificavel e um contato operacional, em linha com a documentacao publica do provider.

## Regras operacionais

- segredos nao devem ficar hardcoded em `appsettings.json`
- `appsettings.json` pode conter valores de desenvolvimento local nao sensiveis
- para ambientes reais, usar variaveis de ambiente ou secret store
- a API deve aceitar `DATABASE_URL` tanto no runtime quanto em comandos de migration

## Estrategia de banco

Usar EF Core Migrations.

Recomendacoes:

- migrations revisadas em codigo
- aplicar migrations pendentes automaticamente na subida da API por padrao
- permitir desligar esse comportamento com `Database__ApplyMigrationsOnStartup=false` quando o ambiente exigir controle externo
- nao depender de criacao manual do schema

## Fluxo local esperado

1. subir PostgreSQL com Docker
2. preencher `.env` com valores locais ou de homologacao
3. subir a API com as variaveis de ambiente corretas
4. deixar a API aplicar migrations pendentes no startup
5. acessar Swagger

## Convencoes operacionais da fundacao

- a API deve expor porta HTTP configuravel por variavel de ambiente
- o container final deve executar apenas o artefato publicado
- o `docker-compose` deve declarar dependencia do PostgreSQL com `healthcheck`
- a string de conexao usada pela API deve apontar para o servico `postgres` quando executada via compose

## Exemplo de servicos no compose

- `api`
- `postgres`

## Requisitos do container da API

- build multi-stage
- imagem final enxuta
- portas configuraveis
- leitura de variaveis de ambiente

## Requisitos do PostgreSQL

- volume persistente
- usuario, senha e database configuraveis
- healthcheck quando possivel

## Execucao local com Docker Compose

Passos recomendados:

1. criar `.env` a partir de `.env.example`
2. definir `Jwt__Key` com pelo menos 32 caracteres
3. executar `docker compose up --build`
4. acessar `http://localhost:8080/swagger`

## Execucao com banco remoto

Quando o banco estiver fora do compose:

1. definir `DATABASE_URL`
2. iniciar a API com `dotnet run --project src/MeuAcervo.API`

Observacao:

- se houver necessidade operacional de aplicar migrations fora da API, pode-se desabilitar o startup automatico com `Database__ApplyMigrationsOnStartup=false` e executar `dotnet ef database update --project src/MeuAcervo.Infrastructure --startup-project src/MeuAcervo.API`

## Caminho futuro de deploy

O projeto deve poder ser publicado em:

- VPS
- container host
- orquestradores como Kubernetes no futuro
- plataformas de container compativeis

## Swagger e documentacao operacional

Ao subir localmente, a API deve expor Swagger para facilitar integracao com frontend e testes manuais.
