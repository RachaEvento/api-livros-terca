# Roadmap

## Objetivo

Organizar a implementação em entregas incrementais, mantendo a disciplina de especificação antes do código.

## Fase 1 - Fundação

- estruturar solution e projetos
- configurar API, Application, Domain e Infrastructure
- configurar PostgreSQL
- configurar EF Core
- configurar Swagger
- configurar middleware global
- configurar autenticação JWT

## Fase 2 - Identidade e tenancy

- registro de usuário
- login
- refresh token
- tenant pessoal automático
- roles e permissões iniciais

## Fase 3 - Catálogo e providers

- modelar `BookWork`, `BookEdition`, `Author` e referências externas
- integrar Open Library
- implementar busca unificada
- importar edição para o catálogo interno

## Fase 4 - Acervo do usuário

- CRUD de `UserLibraryItem`
- wishlist
- status de leitura
- progresso
- favoritos
- filtros, ordenação e paginação

## Fase 5 - Organização avançada

- tags
- custom fields
- reviews
- empréstimos

## Fase 6 - Perfil público

- username público
- estatísticas
- favoritos públicos
- reviews públicas
- atividade recente pública

## Fase 7 - Robustez operacional

- índices finais
- melhoria de logs
- otimizações de performance
- cache de integrações externas
- endurecimento de segurança

## Regras do roadmap

- nenhuma fase começa sem revisar os documentos relevantes
- mudanças de escopo exigem atualização da documentação
- preferir entregas verticais pequenas e completas

## Backlog futuro

- múltiplos providers de livros
- tenants compartilhados
- seguidores
- feed de atividade
- notificações
- recomendações
- background jobs
- mensageria e eventos
