# Arquitetura de Livros

## Problema

Livros não podem ser tratados como um registro plano porque:

- uma obra possui múltiplas edições
- provedores externos devolvem metadados incompletos e heterogêneos
- o usuário pode ter várias cópias ou formatos
- o acervo possui dados próprios que não pertencem ao catálogo global

## Modelo conceitual

### `BookWork`

Representa a obra intelectual.

Exemplos:

- título canônico
- descrição
- autores principais
- ano de publicação original

### `BookEdition`

Representa a edição específica.

Exemplos:

- ISBN
- editora
- idioma
- número de páginas
- ano da edição
- capa
- número da edição

### `UserLibraryItem`

Representa a relação do usuário com uma edição.

Exemplos:

- status de leitura
- progresso
- nota
- favorito
- localização física
- condição
- empréstimos
- custom fields

## Benefícios desta separação

- evita duplicidade desnecessária
- permite coexistência de várias edições
- prepara o sistema para múltiplos providers
- facilita busca e deduplicação
- isola o catálogo dos dados do usuário

## Regras de domínio

- toda entrada no acervo referencia uma `BookEdition`
- uma `BookEdition` sempre referencia uma `BookWork`
- o usuário nunca grava metadados bibliográficos diretamente no catálogo global
- importação externa cria ou reconcilia `BookWork` e `BookEdition`
- reconciliação externa deve tentar reaproveitar `Author`, `Publisher` e `ExternalBookReference` existentes antes de criar novos registros
- o catálogo global pode ser enriquecido por importações repetidas da mesma edição sem duplicar a estrutura base

## Estratégia de deduplicação

Prioridades recomendadas:

1. ISBN-13
2. ISBN-10
3. combinação normalizada de título + autor + editora + idioma

Campos normalizados recomendados para apoiar reconciliação:

- `BookWork.NormalizedCanonicalTitle`
- `BookEdition.NormalizedTitle`
- `Author.NormalizedName`
- `Publisher.NormalizedName`

## Metadados mínimos para edição

Idealmente:

- título
- obra vinculada
- idioma
- pelo menos um identificador externo ou ISBN

Quando o provider não entregar todos os campos, permitir edição parcial com posterior enriquecimento.

## Estados do item do acervo

- `Collection`
- `Wishlist`
- `Archived`

E o status de leitura segue fluxo separado:

- `NotStarted`
- `Reading`
- `Paused`
- `Completed`
- `Abandoned`
- `Rereading`

## Ajustes futuros previstos

- múltiplos contribuidores com papéis diferentes
- séries e coleções editoriais
- suporte a volumes
- enriquecimento assíncrono do catálogo

## Rastreabilidade externa

O catálogo interno deve manter `ExternalBookReference` para preservar vínculo com providers sem acoplamento do restante do domínio.

Regras recomendadas:

- referência pode apontar para `BookWork` ou `BookEdition`
- `Provider + ExternalId + ReferenceType` identifica a origem de forma única
- a reconciliação externa nunca deve substituir a separação entre obra e edição
