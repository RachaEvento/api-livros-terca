# Autenticação JWT

## Objetivo

Fornecer autenticação segura, simples de consumir por frontend web/mobile e preparada para renovação de sessão via refresh token.

## Fluxos obrigatórios

- registro de usuário
- login
- refresh token
- logout
- revogação de refresh token

## Estratégia

- **Access token** curto, assinado e usado em chamadas autenticadas
- **Refresh token** longo, persistido com hash no banco
- rotação de refresh token a cada renovação de sessão

## Claims mínimas do access token

- `sub`: id do usuário
- `tid`: id do tenant
- `email`
- `username`
- `roles`
- `jti`

## Configuração sugerida

- Access token: 15 minutos
- Refresh token: 30 dias
- Assinatura com chave forte em variável de ambiente
- `issuer` e `audience` configuráveis

## Regras mínimas de credencial na v1

- senha com mínimo de 8 caracteres
- exigir pelo menos 1 letra maiúscula
- exigir pelo menos 1 letra minúscula
- exigir pelo menos 1 número
- exigir pelo menos 1 caractere não alfanumérico

Essas regras podem evoluir depois, mas a v1 deve ser explícita e determinística.

## Endpoints sugeridos

### `POST /api/v1/auth/register`

Cria usuário e tenant pessoal.

Contrato esperado:

- recebe `email`, `username`, `displayName`, `password`
- valida unicidade global de `email` e `username`
- cria tenant pessoal
- cria usuário ativo
- cria e atribui papel `Owner`
- retorna sessão autenticada já aberta com access token, refresh token, resumo do usuário, resumo do tenant, roles e permissões

### `POST /api/v1/auth/login`

Valida credenciais e retorna:

- access token
- refresh token
- dados básicos do usuário
- dados básicos do tenant
- roles
- permissões efetivas da sessão

Contrato esperado:

- recebe `emailOrUsername` e `password`
- autentica por email ou username normalizado

### `POST /api/v1/auth/refresh`

Recebe refresh token válido, rotaciona a credencial e emite novo access token.

Contrato esperado:

- recebe apenas o refresh token bruto
- valida hash, expiração e revogação
- revoga o token atual
- cria novo refresh token persistido
- retorna novo access token, novo refresh token, usuário, tenant, roles e permissões da sessão
- reutilização de token revogado ou rotacionado deve falhar

### `POST /api/v1/auth/logout`

Revoga o refresh token informado ou o conjunto da sessão atual.

Contrato mínimo da v1:

- recebe o refresh token bruto a ser revogado
- revoga apenas a sessão correspondente ao token informado
- exige usuário autenticado para impedir revogação arbitrária entre usuários

## Persistência de refresh token

Campos sugeridos:

- `Id`
- `TenantId`
- `UserId`
- `TokenHash`
- `JwtId`
- `ExpiresAtUtc`
- `RevokedAtUtc`
- `CreatedAtUtc`
- `CreatedByIp`
- `ReplacedByTokenHash`

## Regras de segurança

- nunca armazenar refresh token em texto puro
- invalidar token revogado
- impedir reutilização de refresh token rotacionado
- registrar `jti` para rastreabilidade
- considerar bloqueio após múltiplas tentativas falhas no futuro
- a comparação do token informado deve ocorrer sobre hash derivado do valor bruto recebido
- logout e refresh devem sempre validar correspondência entre token, usuário e tenant da sessão quando houver contexto autenticado

## Registro de usuário

O fluxo de registro deve:

1. validar email e username únicos
2. criar tenant pessoal
3. criar usuário
4. vincular papel padrão de `Owner`
5. opcionalmente retornar sessão autenticada já aberta

## Integração com autorização

O JWT fornece contexto para:

- identificar usuário
- resolver tenant
- aplicar políticas de autorização

Na v1:

- roles podem ser emitidos no JWT
- permissões detalhadas devem ser resolvidas por banco ou cache local por tenant
- não confiar em permissões enviadas pelo cliente

## Documentação OpenAPI

Swagger deve expor:

- esquema `Bearer`
- exemplos de payloads
- indicação clara de endpoints públicos e protegidos
- requisito de segurança por operação nos endpoints protegidos, para que o Swagger UI envie `Authorization: Bearer {token}` automaticamente após o uso do botão Authorize
