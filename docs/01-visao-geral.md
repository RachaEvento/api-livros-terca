# Visao Geral

## Proposito

**Meu Acervo** e uma plataforma de gerenciamento de biblioteca pessoal inspirada em produtos como Goodreads, MyAnimeList e Letterboxd, mas especializada em **livros fisicos e digitais**. O sistema deve ir alem de um CRUD simples e permitir organizacao, leitura, avaliacao, catalogacao e compartilhamento parcial do acervo de cada usuario.

## Objetivo do produto

Entregar uma API REST profissional para:

- organizar acervos pessoais com isolamento por tenant
- pesquisar livros em fontes externas
- registrar edicoes especificas e formatos distintos
- acompanhar progresso de leitura
- armazenar avaliacoes, resenhas, emprestimos e campos customizados opcionais
- expor parcialmente um perfil publico do leitor

## Persona principal

- leitor individual com biblioteca propria
- usuario que registra colecao fisica e digital
- usuario que deseja historico de leitura e organizacao detalhada

## Escopo funcional inicial

O usuario podera:

- criar conta e autenticar-se com JWT
- manter um acervo pessoal isolado
- adicionar livros ao acervo ou wishlist
- registrar status de leitura
- registrar progresso de leitura
- favoritar livros
- avaliar livros de 1 a 5 estrelas
- escrever resenhas e notas
- cadastrar campos customizados
- controlar emprestimos
- consultar perfil publico parcial
- pesquisar livros externos inicialmente via Open Library

## Status de leitura suportados

- `NotStarted`
- `Reading`
- `Paused`
- `Completed`
- `Abandoned`
- `Rereading`

## Formatos de aquisicao suportados no item

- `Physical`
- `Ebook`
- `Audiobook`
- `Other`

## Principios de dominio

- o sistema nao trata livro como registro unico e simples
- uma mesma obra pode ter multiplas edicoes coexistindo
- o catalogo bibliografico deve ser separado dos dados de uso do usuario
- o acervo do usuario e o centro da experiencia
- a arquitetura deve suportar expansao para fontes externas adicionais e funcionalidades sociais futuras

## Objetivos de qualidade

- escalabilidade
- modularidade
- extensibilidade
- seguranca
- performance
- manutenibilidade
- desacoplamento de frontend

## Fora de escopo da v1

- seguidores
- chat
- feed social completo
- recomendacao inteligente
- sincronizacao automatica com dispositivos de leitura
- testes automatizados obrigatorios nesta fase inicial

## Restricoes tecnologicas

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

1. Registrar e autenticar usuario.
2. Pesquisar livro em fonte externa.
3. Importar ou vincular edicao ao catalogo interno.
4. Adicionar item ao acervo pessoal.
5. Marcar status e progresso de leitura.
6. Avaliar, comentar e favoritar um item.
7. Definir metadados customizados para o tenant.
8. Registrar emprestimo de livro fisico.
9. Compartilhar parte do perfil publicamente.

## Diretriz de implementacao

Toda funcionalidade deve nascer de especificacao clara em `docs/`, com contratos explicitos, regras de dominio definidas e atencao especial a:

- multi-tenancy
- consistencia do catalogo
- autenticacao e autorizacao
- paginacao e filtros
- resposta padronizada
- performance de consultas
