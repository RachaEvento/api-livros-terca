# Autenticacao JWT

## Objetivo

Fornecer autenticacao segura e simples de consumir por frontend web/mobile usando apenas JWT de acesso na v1 atual.

## Fluxos obrigatorios

- registro de usuario
- login
- consulta da sessao atual
- logout local no cliente

## Estrategia

- **Access token** curto, assinado e usado em chamadas autenticadas
- sessao sem estado no servidor para manter a implementacao simples
- nova autenticacao exigida quando o token expirar

## Claims minimas do access token

- `sub`: id do usuario
- `tid`: id do tenant
- `email`
- `username`
- `roles`
- `jti`

## Configuracao sugerida

- Access token: 15 minutos
- Assinatura com chave forte em variavel de ambiente
- `issuer` e `audience` configuraveis

## Regras minimas de credencial na v1

- senha com minimo de 8 caracteres
- exigir pelo menos 1 letra maiuscula
- exigir pelo menos 1 letra minuscula
- exigir pelo menos 1 numero
- exigir pelo menos 1 caractere nao alfanumerico

Essas regras podem evoluir depois, mas a v1 deve ser explicita e deterministica.

## Endpoints sugeridos

### `POST /api/v1/auth/register`

Cria usuario e tenant pessoal.

Contrato esperado:

- recebe `email`, `username`, `displayName`, `password`
- valida unicidade global de `email` e `username`
- cria tenant pessoal
- cria usuario ativo
- cria e atribui papel `Owner`
- retorna sessao autenticada ja aberta com access token, expiracao do access token, resumo do usuario, resumo do tenant, roles e permissoes

### `POST /api/v1/auth/login`

Valida credenciais e retorna:

- access token
- instante de expiracao do access token
- dados basicos do usuario
- dados basicos do tenant
- roles
- permissoes efetivas da sessao

Contrato esperado:

- recebe `emailOrUsername` e `password`
- autentica por email ou username normalizado

### `GET /api/v1/auth/me`

Retorna os dados da sessao autenticada atual.

Contrato esperado:

- exige `Authorization: Bearer {token}`
- retorna usuario, tenant, roles e permissoes da sessao valida

### `POST /api/v1/auth/logout`

Encerramento de sessao no modelo simplificado atual.

Contrato minimo da v1:

- nao exige payload
- pode apenas retornar confirmacao para o cliente descartar o token local
- deve ser idempotente para simplificar integracao

## Regras de seguranca

- registrar `jti` para rastreabilidade
- considerar bloqueio apos multiplas tentativas falhas no futuro
- o cliente deve remover o token local ao receber `401 Unauthorized`
- como nao ha revogacao server-side na v1 simplificada, o tempo de vida do access token deve permanecer curto

## Registro de usuario

O fluxo de registro deve:

1. validar email e username unicos
2. criar tenant pessoal
3. criar usuario
4. vincular papel padrao de `Owner`
5. opcionalmente retornar sessao autenticada ja aberta

## Integracao com autorizacao

O JWT fornece contexto para:

- identificar usuario
- resolver tenant
- aplicar politicas de autorizacao

Na v1:

- roles podem ser emitidos no JWT
- permissoes detalhadas devem ser resolvidas por banco ou cache local por tenant
- nao confiar em permissoes enviadas pelo cliente

## Documentacao OpenAPI

Swagger deve expor:

- esquema `Bearer`
- exemplos de payloads
- indicacao clara de endpoints publicos e protegidos
- requisito de seguranca por operacao nos endpoints protegidos, para que o Swagger UI envie `Authorization: Bearer {token}` automaticamente apos o uso do botao Authorize
