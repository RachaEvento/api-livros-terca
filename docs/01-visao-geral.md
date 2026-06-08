# Visão Geral

## Propósito

**Meu Acervo** é uma plataforma de gerenciamento de biblioteca pessoal inspirada em produtos como Goodreads, MyAnimeList e Letterboxd, mas especializada em **livros físicos e digitais**. O sistema deve ir além de um CRUD simples e permitir organização, leitura, avaliação, catalogação e compartilhamento parcial do acervo de cada usuário.

## Objetivo do produto

Entregar uma API REST profissional para:

- organizar acervos pessoais com isolamento por tenant
- pesquisar livros em fontes externas
- registrar edições específicas e formatos distintos
- acompanhar progresso de leitura
- armazenar avaliações, resenhas, tags e empréstimos
- expor parcialmente um perfil público do leitor

## Persona principal

- Leitor individual com biblioteca própria
- Usuário que registra coleção física e digital
- Usuário que deseja histórico de leitura e organização detalhada

## Escopo funcional inicial

O usuário poderá:

- criar conta e autenticar-se com JWT
- manter um acervo pessoal isolado
- adicionar livros ao acervo ou wishlist
- registrar status de leitura
- registrar progresso de leitura
- favoritar livros
- avaliar livros de 1 a 5 estrelas
- escrever resenhas e notas
- criar tags personalizadas
- cadastrar campos customizados
- controlar empréstimos
- consultar perfil público parcial
- pesquisar livros externos inicialmente via Open Library

## Status de leitura suportados

- `NotStarted`
- `Reading`
- `Paused`
- `Completed`
- `Abandoned`
- `Rereading`

## Formatos de item suportados

- `Physical`
- `Kindle`
- `Pdf`
- `Audiobook`
- `Manga`
- `Comic`

## Princípios de domínio

- O sistema não trata livro como registro único e simples.
- Uma mesma obra pode ter múltiplas edições coexistindo.
- O catálogo bibliográfico deve ser separado dos dados de uso do usuário.
- O acervo do usuário é o centro da experiência.
- A arquitetura deve suportar expansão para fontes externas adicionais e funcionalidades sociais futuras.

## Objetivos de qualidade

- escalabilidade
- modularidade
- extensibilidade
- segurança
- performance
- manutenibilidade
- desacoplamento de frontend

## Fora de escopo da v1

- seguidores
- chat
- feed social completo
- recomendação inteligente
- sincronização automática com dispositivos de leitura
- testes automatizados obrigatórios nesta fase inicial

## Restrições tecnológicas

- ASP.NET Core Web API
- .NET 10 preferencialmente
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- Swagger/OpenAPI
- FluentValidation
- AutoMapper
- Docker e docker-compose

## Casos de uso principais

1. Registrar e autenticar usuário.
2. Pesquisar livro em fonte externa.
3. Importar ou vincular edição ao catálogo interno.
4. Adicionar item ao acervo pessoal.
5. Marcar status e progresso de leitura.
6. Avaliar, comentar, favoritar e etiquetar um item.
7. Definir metadados customizados para o tenant.
8. Registrar empréstimo de livro físico.
9. Compartilhar parte do perfil publicamente.

## Diretriz de implementação

Toda funcionalidade deve nascer de especificação clara em `docs/`, com contratos explícitos, regras de domínio definidas e atenção especial a:

- multi-tenancy
- consistência do catálogo
- autenticação e autorização
- paginação e filtros
- resposta padronizada
- performance de consultas
