# Roadmap

## Objetivo

Organizar a implementacao em entregas incrementais, mantendo a disciplina de especificacao antes do codigo.

## Fase 1 - Fundacao

- estruturar solution e projetos
- configurar API, Application, Domain e Infrastructure
- configurar PostgreSQL
- configurar EF Core
- configurar Swagger
- configurar middleware global
- configurar autenticacao JWT

## Fase 2 - Identidade e tenancy

- registro de usuario
- login
- renovacao server-side de sessao, se a simplicidade atual deixar de atender
- tenant pessoal automatico
- roles e permissoes iniciais

## Fase 3 - Catalogo bibliografico interno

- modelar `BookWork`, `BookEdition`, `Author` e referencias externas
- preparar indices e deduplicacao estrutural por ISBN
- separar catalogo global do acervo tenant-scoped

## Fase 4 - Providers de livros

- integrar Open Library
- implementar busca unificada
- importar edicao para o catalogo interno
- manter arquitetura pronta para novos providers

## Fase 5 - Acervo do usuario

- CRUD de `UserLibraryItem`
- wishlist
- status de leitura
- progresso
- favoritos
- filtros, ordenacao e paginacao

## Fase 6 - Organizacao avancada

- custom fields
- reviews
- emprestimos

## Fase 7 - Perfil publico e acabamento operacional

- username publico
- estatisticas
- favoritos publicos
- reviews publicas
- atividade recente publica
- indices finais
- melhoria de documentacao operacional
- revisao de deploy e configuracao

## Regras do roadmap

- nenhuma fase comeca sem revisar os documentos relevantes
- mudancas de escopo exigem atualizacao da documentacao
- preferir entregas verticais pequenas e completas

## Backlog futuro

- biblioteca publica paginada
- multiplos providers de livros
- tenants compartilhados
- seguidores
- feed de atividade completo
- notificacoes
- recomendacoes
- background jobs
- mensageria e eventos
