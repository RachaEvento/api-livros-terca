# Entidades

## Visão geral

O sistema é dividido em quatro áreas principais de dados:

- identidade e acesso
- catálogo bibliográfico global
- acervo do usuário
- customização e social

## Entidades principais

### Identidade e acesso

| Entidade | Escopo | Finalidade |
| --- | --- | --- |
| `Tenant` | tenant | representa o espaço isolado de dados |
| `User` | tenant/global controlado | usuário autenticável |
| `Role` | tenant | papel de autorização |
| `Permission` | global | catálogo de permissões |
| `UserRole` | tenant | vínculo entre usuário e papel |
| `RolePermission` | tenant/global | vínculo entre papel e permissão |

### Catálogo bibliográfico

| Entidade | Escopo | Finalidade |
| --- | --- | --- |
| `Author` | global | autor ou contribuidor |
| `Publisher` | global | editora |
| `BookWork` | global | obra conceitual |
| `BookEdition` | global | edição específica de uma obra |
| `BookEditionAuthor` | global | relacionamento N:N edição-autor |
| `ExternalBookReference` | global | ids e rastreabilidade por provider |

### Acervo do usuário

| Entidade | Escopo | Finalidade |
| --- | --- | --- |
| `UserLibraryItem` | tenant | item do acervo ou wishlist |
| `ReadingProgressEntry` | tenant | histórico de progresso |
| `Review` | tenant | avaliação, resenha ou nota publicada |
| `Loan` | tenant | empréstimo de item físico |

### Customização e perfil

| Entidade | Escopo | Finalidade |
| --- | --- | --- |
| `CustomFieldDefinition` | tenant | definição de campo customizado |
| `CustomFieldOption` | tenant | opções de campo do tipo lista |
| `CustomFieldValue` | tenant | valor aplicado a uma entidade |
| `UserProfile` | tenant/public | configurações de perfil público |

## Modelagem recomendada

### Tenant

Campos sugeridos:

- `Id`
- `Name`
- `Slug`
- `Type`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Observação:

- Na v1, cada usuário recebe um tenant pessoal.
- O modelo deve aceitar tenants de grupo futuramente.

### User

Campos sugeridos:

- `Id`
- `TenantId`
- `Email`
- `NormalizedEmail`
- `Username`
- `NormalizedUsername`
- `PasswordHash`
- `DisplayName`
- `AvatarUrl`
- `Bio`
- `IsActive`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restrições:

- `Email` único globalmente
- `Username` único globalmente

Observações:

- a persistência pode manter campos normalizados para garantir unicidade case-insensitive de email e username
- o valor exibido ao usuário pode permanecer no formato original informado no cadastro

### BookWork

Representa a obra abstrata.

Campos sugeridos:

- `Id`
- `CanonicalTitle`
- `NormalizedCanonicalTitle`
- `OriginalTitle`
- `Description`
- `FirstPublicationYear`
- `PrimaryLanguage`
- `CreatedAtUtc`
- `UpdatedAtUtc`

### BookEdition

Representa uma edição específica vinculada à obra.

Campos sugeridos:

- `Id`
- `BookWorkId`
- `Isbn10`
- `Isbn13`
- `Title`
- `NormalizedTitle`
- `Subtitle`
- `PublisherId`
- `PublishedAt`
- `PageCount`
- `Language`
- `FormatDescriptor`
- `CoverImageUrl`
- `EditionNumber`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restrições:

- índice único parcial para `Isbn13` quando não nulo
- índice único parcial para `Isbn10` quando não nulo
- índice para `BookWorkId`

### Author

Campos sugeridos:

- `Id`
- `Name`
- `NormalizedName`
- `Bio`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restrições:

- índice único em `NormalizedName`

### Publisher

Campos sugeridos:

- `Id`
- `Name`
- `NormalizedName`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restrições:

- índice único em `NormalizedName`

### BookEditionAuthor

Campos sugeridos:

- `Id`
- `BookEditionId`
- `AuthorId`
- `ContributionOrder`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restrições:

- unicidade em `BookEditionId + AuthorId`

### ExternalBookReference

Campos sugeridos:

- `Id`
- `Provider`
- `ExternalId`
- `ReferenceType`
- `BookWorkId`
- `BookEditionId`
- `ExternalUrl`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Regras:

- a referência deve apontar para uma obra ou para uma edição
- `ReferenceType` deve refletir o alvo vinculado
- o par `Provider + ExternalId + ReferenceType` deve ser único

### UserLibraryItem

É o centro do domínio do acervo.

Campos sugeridos:

- `Id`
- `TenantId`
- `UserId`
- `BookEditionId`
- `ShelfType`
- `ReadingStatus`
- `AcquisitionFormat`
- `OwnershipType`
- `IsFavorite`
- `CurrentPage`
- `ProgressPercent`
- `ReadCount`
- `StartedAt`
- `FinishedAt`
- `AcquiredAt`
- `PhysicalLocation`
- `Condition`
- `PrivateNotes`
- `IsDeleted`
- `DeletedAtUtc`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Restrições:

- um usuário pode ter mais de um item para a mesma edição se representar cópias distintas
- se houver suporte a múltiplas cópias, usar um campo como `CopyLabel` ou `CopyNumber`

### Review

Campos sugeridos:

- `Id`
- `TenantId`
- `UserId`
- `UserLibraryItemId`
- `Rating`
- `Title`
- `Content`
- `Visibility`
- `ContainsSpoilers`
- `PublishedAtUtc`
- `CreatedAtUtc`
- `UpdatedAtUtc`

Regra:

- uma review ativa por item por usuário na v1

### Loan

Campos sugeridos:

- `Id`
- `TenantId`
- `UserLibraryItemId`
- `BorrowerName`
- `BorrowerContact`
- `LoanedAtUtc`
- `DueAtUtc`
- `ReturnedAtUtc`
- `Status`
- `Notes`
- `CreatedAtUtc`
- `UpdatedAtUtc`

## Relacionamentos principais

- `Tenant 1:N Users`
- `Tenant 1:N Roles`
- `User N:N Roles`
- `Role N:N Permissions`
- `BookWork 1:N BookEditions`
- `BookEdition N:N Authors`
- `User 1:N UserLibraryItems`
- `BookEdition 1:N UserLibraryItems`
- `UserLibraryItem 1:N ReadingProgressEntries`
- `UserLibraryItem 1:0..1 Review`
- `UserLibraryItem 1:N Loans`
- `CustomFieldDefinition 1:N CustomFieldOptions`
- `CustomFieldDefinition 1:N CustomFieldValues`

## Auditoria e exclusão lógica

Aplicar auditoria básica em todas as entidades relevantes.

Soft delete recomendado para:

- `UserLibraryItem`
- `Review` se a estratégia de negócio exigir recuperação
- `CustomFieldDefinition`

## Observação importante

O catálogo bibliográfico é global e normalizado. O acervo, reviews, empréstimos e campos customizados são dados do tenant e nunca podem vazar entre usuários.
