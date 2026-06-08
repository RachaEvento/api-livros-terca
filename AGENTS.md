# AGENTS.md

## Missão deste repositório

Este repositório usa **desenvolvimento orientado por especificação com apoio de IA**. O objetivo não é apenas gerar código, mas construir um backend profissional para o sistema **Meu Acervo** com base em contratos claros, decisões arquiteturais rastreáveis e documentação viva.

O produto alvo é uma **ASP.NET Core Web API com PostgreSQL**, multi-tenant, extensível e preparada para futuras integrações externas de livros, cache, mensageria e expansão social.

## Ordem obrigatória de leitura

Antes de iniciar qualquer tarefa, leia nesta ordem:

1. `docs/01-visao-geral.md`
2. `docs/02-arquitetura.md`
3. `docs/03-convencoes.md`
4. `docs/04-entidades.md`
5. `docs/05-multi-tenancy.md`
6. Os documentos específicos do módulo que será alterado

Se houver conflito entre código e documentação, trate a documentação como a fonte de verdade temporária até a divergência ser resolvida explicitamente.

## Modo de trabalho: Spec-Driven Development com IA

Toda mudança deve seguir este fluxo:

1. **Entender a necessidade**
   - Identificar o objetivo de negócio.
   - Mapear quais documentos são impactados.
   - Declarar as premissas adotadas quando o requisito não estiver completo.

2. **Atualizar ou confirmar a especificação**
   - Ajustar os arquivos em `docs/` antes ou junto com a implementação.
   - Registrar regras de domínio, contratos de API, impactos em segurança, tenancy e performance.
   - Evitar começar pela camada de controller sem que o comportamento esperado esteja claro.

3. **Definir o impacto arquitetural**
   - Quais entidades, serviços, repositórios, DTOs, validadores e endpoints serão afetados.
   - Se a mudança toca dados multi-tenant, autenticação, permissões ou catálogos globais.
   - Se a mudança exige migration, índice ou ajuste de contrato externo.

4. **Implementar por camadas**
   - `Domain`: regras de negócio, entidades, enums e contratos.
   - `Application`: casos de uso, DTOs, validação, mapeamento, serviços.
   - `Infrastructure`: EF Core, providers externos, autenticação, persistência.
   - `API`: controllers, middleware, documentação OpenAPI e composição.

5. **Validar**
   - Compilação.
   - Integridade de DI.
   - Consistência entre documentação, contratos e modelos.
   - Segurança de tenancy e autorização.

6. **Fechar a tarefa**
   - Atualizar a documentação impactada.
   - Registrar decisões relevantes.
   - Confirmar que não foram expostas entidades diretamente nem quebradas regras transversais.

## Princípios inegociáveis

- **Tenant first**: qualquer dado do usuário deve respeitar isolamento por tenant.
- **Controller magro**: sem regra de negócio em controllers.
- **DTOs sempre**: entidades de domínio não devem ser expostas diretamente pela API.
- **Validação explícita**: entradas devem passar por FluentValidation.
- **Busca externa desacoplada**: integrações com livros via providers e contratos internos.
- **Catálogo global + acervo tenant-scoped**: metadados bibliográficos podem ser compartilhados; dados de uso do usuário não.
- **Campos customizados sem migration por campo**: modelagem flexível e escalável.
- **Soft delete quando fizer sentido**: preferir exclusão lógica em dados de negócio do tenant.
- **Auditoria básica em todas as entidades relevantes**: `CreatedAtUtc`, `UpdatedAtUtc`, opcionalmente `DeletedAtUtc`.
- **Preparado para evolução**: evitar decisões que bloqueiem cache, eventos, background jobs ou expansão social.

## Estrutura alvo do projeto

Quando o código for criado, a estrutura deve seguir esta intenção:

```text
src/
  MeuAcervo.API/
  MeuAcervo.Application/
  MeuAcervo.Domain/
  MeuAcervo.Infrastructure/
  MeuAcervo.Shared/
docs/
```

Dentro dos projetos, manter separação clara entre:

- Controllers
- Services / Use Cases
- Repositories
- Entities
- DTOs
- Validators
- Middlewares
- Configurations
- Data
- Auth
- Common
- Exceptions

## Regra de ouro para modelagem de livros

Nunca tratar livro apenas como um registro único e plano.

O domínio deve separar:

- **Obra (`BookWork`)**: conceito intelectual do livro.
- **Edição (`BookEdition`)**: ISBN, capa, idioma, editora, ano, formato editorial.
- **Item do acervo do usuário (`UserLibraryItem`)**: posse, leitura, progresso, notas, favorito, tags, empréstimos e metadados customizados.

Essa separação é obrigatória para suportar múltiplas edições, deduplicação e integrações externas futuras.

## Definition of Ready

Uma tarefa está pronta para implementação quando:

- O objetivo de negócio está claro.
- Os documentos impactados foram identificados.
- As regras de domínio relevantes estão definidas.
- Há entendimento do impacto em tenancy, auth, permissões e persistência.
- O comportamento esperado da API está descrito em nível suficiente.

## Definition of Done

Uma tarefa só está concluída quando:

- A implementação respeita a especificação vigente.
- O código compila.
- Os contratos de entrada e saída estão consistentes.
- As regras de tenant, autorização e validação foram aplicadas.
- A documentação foi atualizada.
- Não há vazamento de detalhes de infraestrutura para a API pública sem necessidade.

## Convenções para agentes de IA

- Sempre começar pela leitura dos documentos base.
- Sempre explicitar premissas quando o requisito não estiver fechado.
- Se a solicitação contradizer a arquitetura vigente, propor ajuste de spec antes de codar.
- Ao criar um novo módulo, adicionar ou atualizar a documentação correspondente em `docs/`.
- Ao alterar comportamento de domínio, revisar também API, erro, segurança e performance.
- Preferir soluções simples hoje, mas com contratos que suportem evolução amanhã.

## Checklist rápido antes de implementar

- O recurso é tenant-scoped ou global?
- Há impacto em roles/permissões?
- Exige migration ou novo índice?
- Precisa aparecer em perfil público?
- Deve aceitar campos customizados?
- Interage com provider externo?
- Exige paginação, filtro e ordenação?
- Pode gerar inconsistência com múltiplas edições do mesmo livro?

## Resultado esperado desta base documental

Os arquivos em `docs/` funcionam como a especificação de referência para implementar:

- autenticação JWT com refresh token
- multi-tenancy seguro
- catálogo bibliográfico normalizado
- acervo pessoal com status, progresso e favoritos
- tags, reviews, empréstimos e perfil público
- campos customizados por tenant
- integrações externas por providers
- API REST padronizada e pronta para frontend web/mobile
