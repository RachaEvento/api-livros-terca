# Permissões

## Objetivo

Implementar autorização baseada em papéis e permissões, compatível com a v1 e pronta para tenants compartilhados no futuro.

## Estratégia adotada

- RBAC com `Role` e `Permission`
- papéis atribuídos por tenant
- policies baseadas em permissões na API
- roles emitidos no JWT para contexto rápido
- permissões resolvidas por tenant no backend

## Papéis iniciais sugeridos

- `Owner`
- `Admin`
- `Member`
- `Viewer`

Na v1, o usuário recém-registrado recebe `Owner` em seu tenant pessoal.

## Permissões sugeridas

- `library.read`
- `library.write`
- `wishlist.read`
- `wishlist.write`
- `reviews.read`
- `reviews.write`
- `loans.read`
- `loans.write`
- `custom-fields.read`
- `custom-fields.write`
- `profile.read`
- `profile.write`
- `admin.roles.manage`

## Políticas da API

Exemplos:

- `CanReadLibrary`
- `CanWriteLibrary`
- `CanManageLoans`
- `CanManageCustomFields`
- `CanManageRoles`

## Regras práticas

- leitura e escrita devem ser separadas
- endpoints públicos não exigem permissão autenticada
- endpoints protegidos devem validar token e tenant
- perfis públicos só exibem conteúdo marcado como visível

## Modelagem recomendada

- `Role`
- `Permission`
- `UserRole`
- `RolePermission`

## Seed mínimo obrigatório da fase

A base deve sair desta fase com:

- catálogo global de permissões iniciais
- papel `Owner` criado para o tenant pessoal do usuário no registro
- associação do papel `Owner` com todas as permissões iniciais da v1

Papéis `Admin`, `Member` e `Viewer` podem ser preparados como constantes ou referência para uso futuro, mas o fluxo obrigatório desta fase é garantir `Owner`.

## Observação de design

Mesmo com um usuário por tenant na v1, manter RBAC desde o início evita retrabalho quando surgirem tenants compartilhados.

## Matriz mínima esperada

| Papel | Biblioteca | Reviews | Loans | Custom Fields | Perfil |
| --- | --- | --- | --- | --- | --- |
| Owner | leitura/escrita | leitura/escrita | leitura/escrita | leitura/escrita | leitura/escrita |
| Admin | leitura/escrita | leitura/escrita | leitura/escrita | leitura/escrita | leitura/escrita |
| Member | leitura/escrita | leitura/escrita | leitura/escrita | leitura limitada | leitura/escrita |
| Viewer | leitura | leitura | leitura | leitura | leitura |

## Recomendação de implementação

- carregar permissões do usuário no login
- emitir roles no JWT
- resolver permissões detalhadas via banco ou cache local
- usar `IAuthorizationHandler` quando a policy exigir lógica adicional

## Mapeamento recomendado entre policy e permissão

- `CanReadLibrary` -> `library.read`
- `CanWriteLibrary` -> `library.write`
- `CanManageLoans` -> `loans.write`
- `CanManageCustomFields` -> `custom-fields.write`
- `CanManageRoles` -> `admin.roles.manage`
