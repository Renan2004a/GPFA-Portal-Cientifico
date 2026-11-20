# 10 — Dicionário de dados

Banco: `GPFA_Portal` (SQL Server). Tipos conforme `database/01-schema.sql`.

Legenda: **PK** chave primária; **FK** chave estrangeira; **UQ** único; **CK** check; **DF** default.

---

## 1. PerfilAcesso

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| PerfilId | INT IDENTITY | Não | PK | Identificador do perfil |
| Nome | NVARCHAR(50) | Não | UQ, CK len≥3 | Administrador, Coordenador ou Pesquisador |
| Descricao | NVARCHAR(255) | Sim | — | Texto explicativo |

## 2. Pessoa

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| PessoaId | INT IDENTITY | Não | PK | Identificador único |
| NomeCompleto | NVARCHAR(150) | Não | CK len≥3 | Nome civil/apresentação |
| Email | NVARCHAR(150) | Não | UQ | E-mail institucional/pessoal |
| TipoPessoa | CHAR(1) | Não | CK ('P','E') | P=Pesquisador, E=Estudante |
| FotoUrl | NVARCHAR(300) | Sim | — | URL ou caminho local da foto |
| FormacaoAcademica | NVARCHAR(200) | Sim | — | Formação resumida |
| AreaAtuacao | NVARCHAR(150) | Sim | — | Área principal |
| MiniBiografia | NVARCHAR(MAX) | Sim | — | Texto de apresentação |
| GoogleScholarUrl | NVARCHAR(300) | Sim | — | Perfil Scholar |
| OrcidUrl | NVARCHAR(100) | Sim | — | Identificador ORCID |
| LattesUrl | NVARCHAR(300) | Sim | — | Currículo Lattes |
| Ativo | BIT | Não | DF 1 | Visível/ativo |
| CriadoEm | DATETIME2(0) | Não | DF SYSDATETIME() | Criação |
| AtualizadoEm | DATETIME2(0) | Não | DF SYSDATETIME() | Última atualização |

## 3. Pesquisador

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| PesquisadorId | INT | Não | PK, FK Pessoa | Mesmo id da pessoa |
| Titulacao | NVARCHAR(80) | Sim | — | Doutor(a), Pós-doutor etc. |
| Instituicao | NVARCHAR(150) | Sim | — | Instituição |
| Departamento | NVARCHAR(150) | Sim | — | Departamento |
| LinhaPesquisa | NVARCHAR(150) | Sim | — | Linha principal |
| DataCadastro | DATE | Não | DF hoje | Entrada no grupo |

## 4. Estudante

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| EstudanteId | INT | Não | PK, FK Pessoa | Mesmo id da pessoa |
| NivelAcademico | NVARCHAR(40) | Não | CK lista | Nível |
| Curso | NVARCHAR(120) | Não | — | Curso |
| Situacao | NVARCHAR(20) | Não | CK, DF 'Ativo' | Ativo/Concluído/Desligado/Trancado |
| DataIngresso | DATE | Não | — | Início |
| DataConclusao | DATE | Sim | CK ≥ ingresso | Fim |
| CoorientadoresExternos | NVARCHAR(500) | Sim | — | Nomes livres |

**CHECK NivelAcademico:** Iniciação Científica, Graduação, Mestrado, Doutorado, Pós-doutorado, Técnico.

## 5. Projeto

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| ProjetoId | INT IDENTITY | Não | PK | Identificador |
| Titulo | NVARCHAR(200) | Não | CK len≥5 | Título |
| Sigla | NVARCHAR(30) | Sim | — | Sigla curta |
| Resumo | NVARCHAR(1000) | Não | — | Resumo público |
| Descricao | NVARCHAR(MAX) | Sim | — | Descrição completa |
| LinhaPesquisa | NVARCHAR(120) | Sim | — | Linha |
| Situacao | NVARCHAR(20) | Não | CK, DF 'Em andamento' | Situação |
| DataInicio | DATE | Não | — | Início |
| DataFimPrevista | DATE | Sim | CK ≥ início | Previsão |
| DataFim | DATE | Sim | CK ≥ início | Término real |
| CoordenadorId | INT | Não | FK Pesquisador | Coordenador único |
| ImagemUrl | NVARCHAR(300) | Sim | — | Capa |
| Destaque | BIT | Não | DF 0 | Exibir na home |
| CriadoEm / AtualizadoEm | DATETIME2(0) | Não | DF | Auditoria |

**CHECK Situacao:** Em andamento, Concluído, Suspenso, Planejamento.

## 6. ParticipacaoProjeto

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| ParticipacaoId | INT IDENTITY | Não | PK | Identificador |
| ProjetoId | INT | Não | FK Projeto | Projeto |
| PesquisadorId | INT | Não | FK Pesquisador | Participante |
| Papel | NVARCHAR(60) | Não | DF | Papel no projeto |
| DataEntrada | DATE | Não | — | Entrada |
| DataSaida | DATE | Sim | CK ≥ entrada | Saída |

**UQ:** (ProjetoId, PesquisadorId). **Trigger:** coordenador não pode ser participante.

## 7. EstudanteProjeto

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| EstudanteProjetoId | INT IDENTITY | Não | PK | Identificador |
| EstudanteId | INT | Não | FK Estudante (CASCADE) | Estudante |
| ProjetoId | INT | Não | FK Projeto | Projeto |
| DataEntrada | DATE | Não | — | Entrada |
| DataSaida | DATE | Sim | CK ≥ entrada | Saída |
| TipoVinculo | NVARCHAR(60) | Sim | — | Bolsa, colaboração etc. |

**UQ:** (EstudanteId, ProjetoId). **Trigger:** último projeto ativo protegido.

## 8. Orientacao

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| OrientacaoId | INT IDENTITY | Não | PK | Identificador |
| EstudanteId | INT | Não | FK Estudante (CASCADE) | Estudante |
| OrientadorId | INT | Não | FK Pesquisador | Orientador/coorientador interno |
| Principal | BIT | Não | DF 1 | Principal? |
| DataInicio | DATE | Não | — | Início |
| DataFim | DATE | Sim | CK ≥ início | Fim (NULL = ativa) |
| Observacao | NVARCHAR(300) | Sim | — | Notas |

**Índices filtrados:** uma principal ativa por estudante; um par ativo por estudante/orientador.

## 9. Publicacao

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| PublicacaoId | INT IDENTITY | Não | PK | Identificador |
| Titulo | NVARCHAR(300) | Não | — | Título |
| Veiculo | NVARCHAR(200) | Não | — | Revista/evento/editora |
| Ano | SMALLINT | Não | CK 1900–2100 | Ano |
| Tipo | NVARCHAR(40) | Não | CK, DF 'Artigo' | Tipo |
| Doi | NVARCHAR(150) | Sim | — | DOI |
| LinkAcesso | NVARCHAR(400) | Sim | — | URL |
| ProjetoId | INT | Sim | FK Projeto | Projeto associado |
| CriadoEm | DATETIME2(0) | Não | DF | Criação |

**CHECK:** DOI ou link obrigatório.

## 10. PublicacaoAutor

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| PublicacaoId | INT | Não | PK, FK Publicacao (CASCADE) | Publicação |
| PesquisadorId | INT | Não | PK, FK Pesquisador | Autor |
| OrdemAutoria | TINYINT | Não | UQ(PublicacaoId, Ordem), CK 1–50 | Ordem |

## 11. DestaquePublicacao

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| DestaqueId | INT IDENTITY | Não | PK | Identificador |
| PesquisadorId | INT | Não | FK Pesquisador | Dono do destaque |
| PublicacaoId | INT | Não | FK Publicacao (CASCADE) | Publicação |
| Ordem | TINYINT | Não | CK 1–5 | Posição |
| CriadoEm | DATETIME2(0) | Não | DF | Criação |

**UQ:** (PesquisadorId, PublicacaoId), (PesquisadorId, Ordem). **Trigger:** máximo 5 e autoria.

## 12. FonteFinanciadora

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| FonteId | INT IDENTITY | Não | PK | Identificador |
| Nome | NVARCHAR(120) | Não | UQ | Nome completo |
| Sigla | NVARCHAR(30) | Sim | — | Sigla |
| Tipo | NVARCHAR(40) | Não | CK | Agência/Fundação/Privada/Internacional/Outra |
| SiteUrl | NVARCHAR(300) | Sim | — | Site |

## 13. Financiamento

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| FinanciamentoId | INT IDENTITY | Não | PK | Identificador |
| ProjetoId | INT | Não | FK Projeto | Projeto |
| FonteId | INT | Não | FK Fonte | Fonte |
| Descricao | NVARCHAR(200) | Não | — | Descrição |
| NumeroProcesso | NVARCHAR(80) | Sim | — | Processo/convênio |
| Valor | DECIMAL(18,2) | Não | CK > 0 | Valor em BRL |
| DataInicio | DATE | Não | — | Início |
| DataFim | DATE | Sim | CK ≥ início | Fim |
| Moeda | CHAR(3) | Não | CK = 'BRL' | Moeda |
| CriadoEm | DATETIME2(0) | Não | DF | Criação |

**UQ filtrado:** (ProjetoId, FonteId, NumeroProcesso) quando processo informado.

## 14. Usuario

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| UsuarioId | INT IDENTITY | Não | PK | Identificador |
| Login | NVARCHAR(60) | Não | UQ | Login |
| NomeCompleto | NVARCHAR(150) | Não | — | Nome |
| Email | NVARCHAR(150) | Não | UQ | E-mail |
| SenhaHash | NVARCHAR(400) | Não | — | PBKDF2 (iterações.salt.hash) |
| PerfilId | INT | Não | FK Perfil | Perfil |
| PesquisadorId | INT | Sim | FK Pesquisador | Vínculo |
| Ativo | BIT | Não | DF 1 | Ativo |
| UltimoAcesso | DATETIME2(0) | Sim | — | Último login |
| TentativasFalhas | INT | Não | DF 0, CK ≥0 | Falhas consecutivas |
| BloqueadoAte | DATETIME2(0) | Sim | — | Bloqueio temporário |
| CriadoEm | DATETIME2(0) | Não | DF | Criação |

## 15. LogAuditoria

| Coluna | Tipo | Nulo | Restrições | Descrição |
|---|---|---|---|---|
| LogId | BIGINT IDENTITY | Não | PK | Identificador |
| UsuarioId | INT | Sim | FK Usuario (SET NULL) | Autor |
| Acao | NVARCHAR(30) | Não | — | Login, Criar, Atualizar, Excluir etc. |
| Entidade | NVARCHAR(60) | Não | — | Entidade afetada |
| EntidadeId | INT | Sim | — | Id do registro |
| Detalhe | NVARCHAR(500) | Sim | — | Contexto |
| DataHora | DATETIME2(0) | Não | DF | Momento |

---

## 16. Views

| View | Finalidade |
|---|---|
| vw_ProjetoResumo | Projeto com coordenador, contadores e total captado |
| vw_PesquisadorResumo | Perfil com contadores e total captado |
| vw_PesquisadorProjeto | Projetos por pesquisador e papel |
| vw_Orientandos | Orientações com classificação Atual/Passado |
| vw_EstudanteResumo | Estudante com orientador atual e projetos |
| vw_PublicacaoDetalhe | Publicação com autores concatenados |
| vw_FinanciamentoPorProjeto | Soma por projeto |
| vw_FinanciamentoPorFonte | Soma por fonte |
| vw_FinanciamentoPorPesquisador | Soma por projeto/pesquisador |
| vw_IndicadoresGerais | Contadores globais |
| vw_ProjetosPorSituacao | Distribuição por situação |
| vw_PublicacoesPorAno | Distribuição anual |
| vw_PublicacoesPorPesquisador | Produtividade por autor |
| vw_OrientandosPorOrientador | Atuais/passados por orientador |
| vw_AlertasIntegridade | Inconsistências detectadas |

## 17. Dados de demonstração

| Entidade | Quantidade |
|---|---|
| Pesquisadores | 6 |
| Estudantes | 13 (10 ativos) |
| Projetos | 6 |
| Publicações | 14 |
| Financiamentos | 12 (R$ 755.000,00) |
| Fontes | 5 |
| Usuários | 3 |

Os dados são fictícios e servem apenas à apresentação acadêmica.
