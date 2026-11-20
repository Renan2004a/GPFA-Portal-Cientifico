# 09 — Modelagem lógica (DER)

O DER é a tradução do MER para tabelas relacionais implementáveis no SQL Server. O script completo está em [`../database/01-schema.sql`](../database/01-schema.sql).

## 1. Convenções

- Chaves primárias `INT IDENTITY` (exceto especializações, cuja PK é também FK).
- Chaves estrangeiras com `NO ACTION` por padrão.
- `NVARCHAR` para textos, `DATE` para datas, `DECIMAL(18,2)` para valores, `BIT` para booleanos.
- `DATETIME2(0)` para auditoria.
- Nomes de constraints explícitos (`PK_`, `FK_`, `UQ_`, `CK_`).
- Índices filtrados para regras parciais.
- Triggers apenas para regras que não podem ser expressas declarativamente.

## 2. Tabelas

### 2.1 PerfilAcesso

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| PerfilId | INT IDENTITY | Não | PK |
| Nome | NVARCHAR(50) | Não | UNIQUE, CHECK len ≥ 3 |
| Descricao | NVARCHAR(255) | Sim | — |

### 2.2 Pessoa

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| PessoaId | INT IDENTITY | Não | PK |
| NomeCompleto | NVARCHAR(150) | Não | CHECK len ≥ 3 |
| Email | NVARCHAR(150) | Não | UNIQUE |
| TipoPessoa | CHAR(1) | Não | CHECK ('P','E') |
| FotoUrl | NVARCHAR(300) | Sim | — |
| FormacaoAcademica | NVARCHAR(200) | Sim | — |
| AreaAtuacao | NVARCHAR(150) | Sim | — |
| MiniBiografia | NVARCHAR(MAX) | Sim | — |
| GoogleScholarUrl | NVARCHAR(300) | Sim | — |
| OrcidUrl | NVARCHAR(100) | Sim | — |
| LattesUrl | NVARCHAR(300) | Sim | — |
| Ativo | BIT | Não | DEFAULT 1 |
| CriadoEm / AtualizadoEm | DATETIME2(0) | Não | DEFAULT SYSDATETIME() |

### 2.3 Pesquisador

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| PesquisadorId | INT | Não | PK, FK → Pessoa |
| Titulacao | NVARCHAR(80) | Sim | — |
| Instituicao | NVARCHAR(150) | Sim | — |
| Departamento | NVARCHAR(150) | Sim | — |
| LinhaPesquisa | NVARCHAR(150) | Sim | — |
| DataCadastro | DATE | Não | DEFAULT hoje |

### 2.4 Estudante

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| EstudanteId | INT | Não | PK, FK → Pessoa |
| NivelAcademico | NVARCHAR(40) | Não | CHECK lista |
| Curso | NVARCHAR(120) | Não | — |
| Situacao | NVARCHAR(20) | Não | CHECK lista, DEFAULT 'Ativo' |
| DataIngresso | DATE | Não | — |
| DataConclusao | DATE | Sim | CHECK ≥ ingresso |
| CoorientadoresExternos | NVARCHAR(500) | Sim | — |

### 2.5 Projeto

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| ProjetoId | INT IDENTITY | Não | PK |
| Titulo | NVARCHAR(200) | Não | CHECK len ≥ 5 |
| Sigla | NVARCHAR(30) | Sim | — |
| Resumo | NVARCHAR(1000) | Não | — |
| Descricao | NVARCHAR(MAX) | Sim | — |
| LinhaPesquisa | NVARCHAR(120) | Sim | — |
| Situacao | NVARCHAR(20) | Não | CHECK lista |
| DataInicio | DATE | Não | — |
| DataFimPrevista | DATE | Sim | CHECK ≥ início |
| DataFim | DATE | Sim | CHECK ≥ início |
| CoordenadorId | INT | Não | FK → Pesquisador |
| ImagemUrl | NVARCHAR(300) | Sim | — |
| Destaque | BIT | Não | DEFAULT 0 |
| CriadoEm / AtualizadoEm | DATETIME2(0) | Não | DEFAULT |

**Cardinalidade:** Pesquisador 1 : N Projeto (coordena); cada projeto tem exatamente um coordenador.

### 2.6 ParticipacaoProjeto

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| ParticipacaoId | INT IDENTITY | Não | PK |
| ProjetoId | INT | Não | FK → Projeto |
| PesquisadorId | INT | Não | FK → Pesquisador |
| Papel | NVARCHAR(60) | Não | DEFAULT 'Pesquisador' |
| DataEntrada | DATE | Não | — |
| DataSaida | DATE | Sim | CHECK ≥ entrada |

**Restrições:** UNIQUE (ProjetoId, PesquisadorId); trigger impede o coordenador.

### 2.7 EstudanteProjeto

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| EstudanteProjetoId | INT IDENTITY | Não | PK |
| EstudanteId | INT | Não | FK → Estudante (CASCADE) |
| ProjetoId | INT | Não | FK → Projeto |
| DataEntrada | DATE | Não | — |
| DataSaida | DATE | Sim | CHECK ≥ entrada |
| TipoVinculo | NVARCHAR(60) | Sim | — |

**Restrições:** UNIQUE (EstudanteId, ProjetoId); trigger de último projeto ativo.

### 2.8 Orientacao

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| OrientacaoId | INT IDENTITY | Não | PK |
| EstudanteId | INT | Não | FK → Estudante (CASCADE) |
| OrientadorId | INT | Não | FK → Pesquisador |
| Principal | BIT | Não | DEFAULT 1 |
| DataInicio | DATE | Não | — |
| DataFim | DATE | Sim | CHECK ≥ início |
| Observacao | NVARCHAR(300) | Sim | — |

**Índices filtrados:**

```sql
CREATE UNIQUE INDEX UX_Orientacao_PrincipalAtiva
ON dbo.Orientacao (EstudanteId)
WHERE Principal = 1 AND DataFim IS NULL;

CREATE UNIQUE INDEX UX_Orientacao_ParAtivo
ON dbo.Orientacao (EstudanteId, OrientadorId)
WHERE DataFim IS NULL;
```

### 2.9 Publicacao

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| PublicacaoId | INT IDENTITY | Não | PK |
| Titulo | NVARCHAR(300) | Não | — |
| Veiculo | NVARCHAR(200) | Não | — |
| Ano | SMALLINT | Não | CHECK 1900–2100 |
| Tipo | NVARCHAR(40) | Não | CHECK lista |
| Doi | NVARCHAR(150) | Sim | — |
| LinkAcesso | NVARCHAR(400) | Sim | — |
| ProjetoId | INT | Sim | FK → Projeto |
| CriadoEm | DATETIME2(0) | Não | DEFAULT |

**CHECK:** `Doi IS NOT NULL OR LinkAcesso IS NOT NULL`.

### 2.10 PublicacaoAutor

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| PublicacaoId | INT | Não | PK, FK → Publicacao (CASCADE) |
| PesquisadorId | INT | Não | PK, FK → Pesquisador |
| OrdemAutoria | TINYINT | Não | UNIQUE(PublicacaoId, Ordem), CHECK 1–50 |

### 2.11 DestaquePublicacao

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| DestaqueId | INT IDENTITY | Não | PK |
| PesquisadorId | INT | Não | FK → Pesquisador |
| PublicacaoId | INT | Não | FK → Publicacao (CASCADE) |
| Ordem | TINYINT | Não | CHECK 1–5 |
| CriadoEm | DATETIME2(0) | Não | DEFAULT |

**Restrições:** UNIQUE(PesquisadorId, PublicacaoId), UNIQUE(PesquisadorId, Ordem), trigger de máximo 5 e autoria.

### 2.12 FonteFinanciadora

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| FonteId | INT IDENTITY | Não | PK |
| Nome | NVARCHAR(120) | Não | UNIQUE |
| Sigla | NVARCHAR(30) | Sim | — |
| Tipo | NVARCHAR(40) | Não | CHECK lista |
| SiteUrl | NVARCHAR(300) | Sim | — |

### 2.13 Financiamento

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| FinanciamentoId | INT IDENTITY | Não | PK |
| ProjetoId | INT | Não | FK → Projeto |
| FonteId | INT | Não | FK → Fonte |
| Descricao | NVARCHAR(200) | Não | — |
| NumeroProcesso | NVARCHAR(80) | Sim | — |
| Valor | DECIMAL(18,2) | Não | CHECK > 0 |
| DataInicio | DATE | Não | — |
| DataFim | DATE | Sim | CHECK ≥ início |
| Moeda | CHAR(3) | Não | CHECK = 'BRL' |
| CriadoEm | DATETIME2(0) | Não | DEFAULT |

**Índice único filtrado:** `(ProjetoId, FonteId, NumeroProcesso) WHERE NumeroProcesso IS NOT NULL`.

### 2.14 Usuario

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| UsuarioId | INT IDENTITY | Não | PK |
| Login | NVARCHAR(60) | Não | UNIQUE |
| NomeCompleto | NVARCHAR(150) | Não | — |
| Email | NVARCHAR(150) | Não | UNIQUE |
| SenhaHash | NVARCHAR(400) | Não | PBKDF2 |
| PerfilId | INT | Não | FK → PerfilAcesso |
| PesquisadorId | INT | Sim | FK → Pesquisador |
| Ativo | BIT | Não | DEFAULT 1 |
| UltimoAcesso | DATETIME2(0) | Sim | — |
| TentativasFalhas | INT | Não | DEFAULT 0, CHECK ≥ 0 |
| BloqueadoAte | DATETIME2(0) | Sim | — |
| CriadoEm | DATETIME2(0) | Não | DEFAULT |

### 2.15 LogAuditoria

| Coluna | Tipo | Nulo | Restrição |
|---|---|---|---|
| LogId | BIGINT IDENTITY | Não | PK |
| UsuarioId | INT | Sim | FK → Usuario `ON DELETE SET NULL` |
| Acao | NVARCHAR(30) | Não | — |
| Entidade | NVARCHAR(60) | Não | — |
| EntidadeId | INT | Sim | — |
| Detalhe | NVARCHAR(500) | Sim | — |
| DataHora | DATETIME2(0) | Não | DEFAULT |

## 3. Índices

| Índice | Tabela | Colunas | Motivo |
|---|---|---|---|
| IX_Orientacao_Orientador | Orientacao | OrientadorId, DataFim | Perfil do orientador |
| UX_Orientacao_PrincipalAtiva | Orientacao | EstudanteId (filtrado) | Principal única |
| UX_Orientacao_ParAtivo | Orientacao | EstudanteId, OrientadorId (filtrado) | Evitar duplicidade ativa |
| IX_Projeto_Coordenador | Projeto | CoordenadorId | Listar projetos do coordenador |
| IX_Projeto_Situacao | Projeto | Situacao | Filtros |
| IX_Participacao_Pesquisador | ParticipacaoProjeto | PesquisadorId, DataSaida | Perfil |
| IX_EstudanteProjeto_Projeto | EstudanteProjeto | ProjetoId, DataSaida | Equipe do projeto |
| IX_Publicacao_Ano | Publicacao | Ano DESC | Filtros e ordenação |
| IX_Publicacao_Projeto | Publicacao | ProjetoId | Publicações do projeto |
| IX_PublicacaoAutor_Pesquisador | PublicacaoAutor | PesquisadorId | Perfil |
| IX_Financiamento_Projeto | Financiamento | ProjetoId | Totais |
| IX_Financiamento_Fonte | Financiamento | FonteId | Relatórios |
| UX_Financiamento_Processo | Financiamento | ProjetoId, FonteId, NumeroProcesso (filtrado) | Evitar duplicidade |
| IX_Usuario_Perfil | Usuario | PerfilId | Administração |
| IX_Log_DataHora | LogAuditoria | DataHora DESC | Dashboard |

## 4. Normalização

### 1FN — atributos atômicos

- Não há listas em células. Participações, vínculos, autores e destaques são tabelas próprias.
- Coorientadores externos são uma exceção deliberada e justificada: a carta permite texto livre; não são entidades do sistema.

### 2FN — dependência total da chave

- Tabelas associativas usam a chave composta ou surrogate e dependem do par completo.
- `ParticipacaoProjeto` depende de projeto + pesquisador; `PublicacaoAutor` depende de publicação + pesquisador.
- Não há atributos dependentes apenas de parte da chave.

### 3FN — sem dependências transitivas

- Dados de pessoa não são repetidos em projetos/participações.
- Nome do coordenador é obtido por join, não armazenado no projeto.
- Nome da fonte é referenciado por FK.
- Total captado é derivado por view, não armazenado (evita inconsistência).

### Desnormalizações conscientes

| Caso | Motivo |
|---|---|
| `Pessoa.TipoPessoa` | Acelera filtros e valida a especialização; mantido coerente por trigger |
| `Publicacao.ProjetoId` | Associação direta opcional; poderia ser N:N, mas a carta trata “publicação associada a projeto” como vínculo simples |
| `CoorientadoresExternos` texto | Flexibilidade explícita da especificação |

## 5. Integridade referencial em operações

| Operação | Comportamento |
|---|---|
| Excluir pesquisador com projeto | Bloqueada (FK) |
| Excluir projeto com publicação/financiamento/equipe | Bloqueada (FK) |
| Excluir fonte com financiamento | Bloqueada (FK) |
| Excluir publicação | Remove autores e destaques (CASCADE) |
| Excluir estudante | Remove orientações/vínculos após desativação (serviço) |
| Excluir usuário | Logs permanecem com `UsuarioId = NULL` |
| Encerrar último projeto de ativo | Bloqueada (trigger) |
| Excluir última principal de ativo | Bloqueada (trigger) |
| Inserir coordenador como participante | Bloqueada (trigger) |
| Sexto destaque | Bloqueada (trigger) |
| Valor ≤ 0 | Bloqueada (CHECK) |

## 6. Scripts

| Script | Função |
|---|---|
| `00-create-database.sql` | Cria o banco |
| `01-schema.sql` | Tabelas, constraints, índices e triggers |
| `02-views.sql` | Views de consulta e relatórios |
| `03-seed.sql` | Dados fictícios de demonstração |
| `04-seed-usuarios.sql` | Usuários com hash PBKDF2 |
| `99-drop-database.sql` | Remove o banco (ambiente de desenvolvimento) |

A execução é orquestrada por [`../scripts/Setup-Database.ps1`](../scripts/Setup-Database.ps1).
