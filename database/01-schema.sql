/* ============================================================================
   GPFA - Portal de Visibilidade Científica e Gestão de Projetos
   Script 01 - Estrutura relacional (tabelas, restrições, índices e triggers)

   Convenções:
   - Chaves primárias surrogate INT IDENTITY.
   - Tabelas associativas explícitas para relacionamentos N:N.
   - FKs com NO ACTION por padrão; exclusões são bloqueadas e tratadas na
     camada de negócio com mensagens compreensíveis.
   - Regras que não podem ser expressas por restrições declarativas são
     garantidas por triggers e/ou transações da camada Business.
   ============================================================================ */
:setvar DbName "GPFA_Portal"
USE [$(DbName)];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ---------------------------------------------------------------------------
   1. Perfis de acesso
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.PerfilAcesso
(
    PerfilId    INT IDENTITY(1,1) NOT NULL,
    Nome        NVARCHAR(50)      NOT NULL,
    Descricao   NVARCHAR(255)     NULL,
    CONSTRAINT PK_PerfilAcesso PRIMARY KEY (PerfilId),
    CONSTRAINT UQ_PerfilAcesso_Nome UNIQUE (Nome),
    CONSTRAINT CK_PerfilAcesso_Nome CHECK (LEN(LTRIM(RTRIM(Nome))) >= 3)
);
GO

/* ---------------------------------------------------------------------------
   2. Pessoa (superclasse) e especializações Pesquisador / Estudante (TPT)
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.Pessoa
(
    PessoaId            INT IDENTITY(1,1) NOT NULL,
    NomeCompleto        NVARCHAR(150)     NOT NULL,
    Email               NVARCHAR(150)     NOT NULL,
    TipoPessoa          CHAR(1)           NOT NULL,
    FotoUrl             NVARCHAR(300)     NULL,
    FormacaoAcademica   NVARCHAR(200)     NULL,
    AreaAtuacao         NVARCHAR(150)     NULL,
    MiniBiografia       NVARCHAR(MAX)     NULL,
    GoogleScholarUrl    NVARCHAR(300)     NULL,
    OrcidUrl            NVARCHAR(100)     NULL,
    LattesUrl           NVARCHAR(300)     NULL,
    Ativo               BIT               NOT NULL CONSTRAINT DF_Pessoa_Ativo DEFAULT (1),
    CriadoEm            DATETIME2(0)      NOT NULL CONSTRAINT DF_Pessoa_CriadoEm DEFAULT (SYSDATETIME()),
    AtualizadoEm        DATETIME2(0)      NOT NULL CONSTRAINT DF_Pessoa_AtualizadoEm DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Pessoa PRIMARY KEY (PessoaId),
    CONSTRAINT UQ_Pessoa_Email UNIQUE (Email),
    CONSTRAINT CK_Pessoa_Tipo CHECK (TipoPessoa IN ('P','E')),
    CONSTRAINT CK_Pessoa_Nome CHECK (LEN(LTRIM(RTRIM(NomeCompleto))) >= 3)
);
GO

CREATE TABLE dbo.Pesquisador
(
    PesquisadorId       INT               NOT NULL,
    Titulacao           NVARCHAR(80)      NULL,
    Instituicao         NVARCHAR(150)     NULL,
    Departamento        NVARCHAR(150)     NULL,
    LinhaPesquisa       NVARCHAR(150)     NULL,
    DataCadastro        DATE              NOT NULL CONSTRAINT DF_Pesquisador_DataCadastro DEFAULT (CAST(SYSDATETIME() AS DATE)),
    CONSTRAINT PK_Pesquisador PRIMARY KEY (PesquisadorId),
    CONSTRAINT FK_Pesquisador_Pessoa FOREIGN KEY (PesquisadorId) REFERENCES dbo.Pessoa (PessoaId)
);
GO

CREATE TABLE dbo.Estudante
(
    EstudanteId             INT               NOT NULL,
    NivelAcademico          NVARCHAR(40)      NOT NULL,
    Curso                   NVARCHAR(120)     NOT NULL,
    Situacao                NVARCHAR(20)      NOT NULL CONSTRAINT DF_Estudante_Situacao DEFAULT (N'Ativo'),
    DataIngresso            DATE              NOT NULL,
    DataConclusao           DATE              NULL,
    CoorientadoresExternos  NVARCHAR(500)     NULL,
    CONSTRAINT PK_Estudante PRIMARY KEY (EstudanteId),
    CONSTRAINT FK_Estudante_Pessoa FOREIGN KEY (EstudanteId) REFERENCES dbo.Pessoa (PessoaId),
    CONSTRAINT CK_Estudante_Nivel CHECK (NivelAcademico IN
        (N'Iniciação Científica', N'Graduação', N'Mestrado', N'Doutorado', N'Pós-doutorado', N'Técnico')),
    CONSTRAINT CK_Estudante_Situacao CHECK (Situacao IN (N'Ativo', N'Concluído', N'Desligado', N'Trancado')),
    CONSTRAINT CK_Estudante_Datas CHECK (DataConclusao IS NULL OR DataConclusao >= DataIngresso)
);
GO

/* ---------------------------------------------------------------------------
   3. Orientação (histórico de orientações, com orientador principal ativo)
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.Orientacao
(
    OrientacaoId    INT IDENTITY(1,1) NOT NULL,
    EstudanteId     INT               NOT NULL,
    OrientadorId    INT               NOT NULL,
    Principal       BIT               NOT NULL CONSTRAINT DF_Orientacao_Principal DEFAULT (1),
    DataInicio      DATE              NOT NULL,
    DataFim         DATE              NULL,
    Observacao      NVARCHAR(300)     NULL,
    CONSTRAINT PK_Orientacao PRIMARY KEY (OrientacaoId),
    CONSTRAINT FK_Orientacao_Estudante FOREIGN KEY (EstudanteId) REFERENCES dbo.Estudante (EstudanteId) ON DELETE CASCADE,
    CONSTRAINT FK_Orientacao_Orientador FOREIGN KEY (OrientadorId) REFERENCES dbo.Pesquisador (PesquisadorId),
    CONSTRAINT CK_Orientacao_Datas CHECK (DataFim IS NULL OR DataFim >= DataInicio)
);
GO

-- Apenas uma orientação principal ativa por estudante (RN04)
CREATE UNIQUE INDEX UX_Orientacao_PrincipalAtiva
    ON dbo.Orientacao (EstudanteId)
    WHERE Principal = 1 AND DataFim IS NULL;
GO

CREATE UNIQUE INDEX UX_Orientacao_ParAtivo
    ON dbo.Orientacao (EstudanteId, OrientadorId)
    WHERE DataFim IS NULL;
GO

CREATE INDEX IX_Orientacao_Orientador ON dbo.Orientacao (OrientadorId, DataFim);
GO

/* ---------------------------------------------------------------------------
   4. Projetos e coordenação (1 coordenador por projeto - RN03)
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.Projeto
(
    ProjetoId       INT IDENTITY(1,1) NOT NULL,
    Titulo          NVARCHAR(200)     NOT NULL,
    Sigla           NVARCHAR(30)      NULL,
    Resumo          NVARCHAR(1000)    NOT NULL,
    Descricao       NVARCHAR(MAX)     NULL,
    LinhaPesquisa   NVARCHAR(120)     NULL,
    Situacao        NVARCHAR(20)      NOT NULL CONSTRAINT DF_Projeto_Situacao DEFAULT (N'Em andamento'),
    DataInicio      DATE              NOT NULL,
    DataFimPrevista DATE              NULL,
    DataFim         DATE              NULL,
    CoordenadorId   INT               NOT NULL,
    ImagemUrl       NVARCHAR(300)     NULL,
    Destaque        BIT               NOT NULL CONSTRAINT DF_Projeto_Destaque DEFAULT (0),
    CriadoEm        DATETIME2(0)      NOT NULL CONSTRAINT DF_Projeto_CriadoEm DEFAULT (SYSDATETIME()),
    AtualizadoEm    DATETIME2(0)      NOT NULL CONSTRAINT DF_Projeto_AtualizadoEm DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Projeto PRIMARY KEY (ProjetoId),
    CONSTRAINT FK_Projeto_Coordenador FOREIGN KEY (CoordenadorId) REFERENCES dbo.Pesquisador (PesquisadorId),
    CONSTRAINT CK_Projeto_Situacao CHECK (Situacao IN (N'Em andamento', N'Concluído', N'Suspenso', N'Planejamento')),
    CONSTRAINT CK_Projeto_Datas CHECK (DataFimPrevista IS NULL OR DataFimPrevista >= DataInicio),
    CONSTRAINT CK_Projeto_DataFim CHECK (DataFim IS NULL OR DataFim >= DataInicio),
    CONSTRAINT CK_Projeto_Titulo CHECK (LEN(LTRIM(RTRIM(Titulo))) >= 5)
);
GO

CREATE INDEX IX_Projeto_Coordenador ON dbo.Projeto (CoordenadorId);
CREATE INDEX IX_Projeto_Situacao ON dbo.Projeto (Situacao);
GO

/* ---------------------------------------------------------------------------
   5. Participação de pesquisadores em projetos (N:N)
      O coordenador é participante implícito e NÃO deve ser duplicado aqui.
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.ParticipacaoProjeto
(
    ParticipacaoId  INT IDENTITY(1,1) NOT NULL,
    ProjetoId       INT               NOT NULL,
    PesquisadorId   INT               NOT NULL,
    Papel           NVARCHAR(60)      NOT NULL CONSTRAINT DF_Participacao_Papel DEFAULT (N'Pesquisador'),
    DataEntrada     DATE              NOT NULL,
    DataSaida       DATE              NULL,
    CONSTRAINT PK_ParticipacaoProjeto PRIMARY KEY (ParticipacaoId),
    CONSTRAINT FK_Participacao_Projeto FOREIGN KEY (ProjetoId) REFERENCES dbo.Projeto (ProjetoId),
    CONSTRAINT FK_Participacao_Pesquisador FOREIGN KEY (PesquisadorId) REFERENCES dbo.Pesquisador (PesquisadorId),
    CONSTRAINT UQ_Participacao_Projeto_Pesquisador UNIQUE (ProjetoId, PesquisadorId),
    CONSTRAINT CK_Participacao_Datas CHECK (DataSaida IS NULL OR DataSaida >= DataEntrada)
);
GO

CREATE INDEX IX_Participacao_Pesquisador ON dbo.ParticipacaoProjeto (PesquisadorId, DataSaida);
GO

/* ---------------------------------------------------------------------------
   6. Vínculo de estudantes com projetos (N:N - RN01)
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.EstudanteProjeto
(
    EstudanteProjetoId INT IDENTITY(1,1) NOT NULL,
    EstudanteId        INT               NOT NULL,
    ProjetoId          INT               NOT NULL,
    DataEntrada        DATE              NOT NULL,
    DataSaida          DATE              NULL,
    TipoVinculo        NVARCHAR(60)      NULL,
    CONSTRAINT PK_EstudanteProjeto PRIMARY KEY (EstudanteProjetoId),
    CONSTRAINT FK_EstudanteProjeto_Estudante FOREIGN KEY (EstudanteId) REFERENCES dbo.Estudante (EstudanteId) ON DELETE CASCADE,
    CONSTRAINT FK_EstudanteProjeto_Projeto FOREIGN KEY (ProjetoId) REFERENCES dbo.Projeto (ProjetoId),
    CONSTRAINT UQ_EstudanteProjeto UNIQUE (EstudanteId, ProjetoId),
    CONSTRAINT CK_EstudanteProjeto_Datas CHECK (DataSaida IS NULL OR DataSaida >= DataEntrada)
);
GO

CREATE INDEX IX_EstudanteProjeto_Projeto ON dbo.EstudanteProjeto (ProjetoId, DataSaida);
GO

/* ---------------------------------------------------------------------------
   7. Publicações, autoria (N:N) e destaques (máx. 5 por pesquisador - RN05)
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.Publicacao
(
    PublicacaoId    INT IDENTITY(1,1) NOT NULL,
    Titulo          NVARCHAR(300)     NOT NULL,
    Veiculo         NVARCHAR(200)     NOT NULL,
    Ano             SMALLINT          NOT NULL,
    Tipo            NVARCHAR(40)      NOT NULL CONSTRAINT DF_Publicacao_Tipo DEFAULT (N'Artigo'),
    Doi             NVARCHAR(150)     NULL,
    LinkAcesso      NVARCHAR(400)     NULL,
    ProjetoId       INT               NULL,
    CriadoEm        DATETIME2(0)      NOT NULL CONSTRAINT DF_Publicacao_CriadoEm DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Publicacao PRIMARY KEY (PublicacaoId),
    CONSTRAINT FK_Publicacao_Projeto FOREIGN KEY (ProjetoId) REFERENCES dbo.Projeto (ProjetoId),
    CONSTRAINT CK_Publicacao_Ano CHECK (Ano BETWEEN 1900 AND 2100),
    CONSTRAINT CK_Publicacao_Tipo CHECK (Tipo IN (N'Artigo', N'Artigo de conferência', N'Livro', N'Capítulo de livro', N'Patente', N'Outro')),
    CONSTRAINT CK_Publicacao_Acesso CHECK (Doi IS NOT NULL OR LinkAcesso IS NOT NULL)
);
GO

CREATE INDEX IX_Publicacao_Ano ON dbo.Publicacao (Ano DESC);
CREATE INDEX IX_Publicacao_Projeto ON dbo.Publicacao (ProjetoId);
GO

CREATE TABLE dbo.PublicacaoAutor
(
    PublicacaoId    INT     NOT NULL,
    PesquisadorId   INT     NOT NULL,
    OrdemAutoria    TINYINT NOT NULL,
    CONSTRAINT PK_PublicacaoAutor PRIMARY KEY (PublicacaoId, PesquisadorId),
    CONSTRAINT FK_PublicacaoAutor_Publicacao FOREIGN KEY (PublicacaoId) REFERENCES dbo.Publicacao (PublicacaoId) ON DELETE CASCADE,
    CONSTRAINT FK_PublicacaoAutor_Pesquisador FOREIGN KEY (PesquisadorId) REFERENCES dbo.Pesquisador (PesquisadorId),
    CONSTRAINT UQ_PublicacaoAutor_Ordem UNIQUE (PublicacaoId, OrdemAutoria),
    CONSTRAINT CK_PublicacaoAutor_Ordem CHECK (OrdemAutoria BETWEEN 1 AND 50)
);
GO

CREATE INDEX IX_PublicacaoAutor_Pesquisador ON dbo.PublicacaoAutor (PesquisadorId);
GO

CREATE TABLE dbo.DestaquePublicacao
(
    DestaqueId      INT IDENTITY(1,1) NOT NULL,
    PesquisadorId   INT     NOT NULL,
    PublicacaoId    INT     NOT NULL,
    Ordem           TINYINT NOT NULL,
    CriadoEm        DATETIME2(0) NOT NULL CONSTRAINT DF_Destaque_CriadoEm DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_DestaquePublicacao PRIMARY KEY (DestaqueId),
    CONSTRAINT FK_Destaque_Pesquisador FOREIGN KEY (PesquisadorId) REFERENCES dbo.Pesquisador (PesquisadorId),
    CONSTRAINT FK_Destaque_Publicacao FOREIGN KEY (PublicacaoId) REFERENCES dbo.Publicacao (PublicacaoId) ON DELETE CASCADE,
    CONSTRAINT UQ_Destaque_Pesquisador_Publicacao UNIQUE (PesquisadorId, PublicacaoId),
    CONSTRAINT UQ_Destaque_Pesquisador_Ordem UNIQUE (PesquisadorId, Ordem),
    CONSTRAINT CK_Destaque_Ordem CHECK (Ordem BETWEEN 1 AND 5)
);
GO

/* ---------------------------------------------------------------------------
   8. Fontes e registros de financiamento (RN06)
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.FonteFinanciadora
(
    FonteId     INT IDENTITY(1,1) NOT NULL,
    Nome        NVARCHAR(120)     NOT NULL,
    Sigla       NVARCHAR(30)      NULL,
    Tipo        NVARCHAR(40)      NOT NULL,
    SiteUrl     NVARCHAR(300)     NULL,
    CONSTRAINT PK_FonteFinanciadora PRIMARY KEY (FonteId),
    CONSTRAINT UQ_FonteFinanciadora_Nome UNIQUE (Nome),
    CONSTRAINT CK_Fonte_Tipo CHECK (Tipo IN (N'Agência pública', N'Fundação', N'Parceria privada', N'Internacional', N'Outra'))
);
GO

CREATE TABLE dbo.Financiamento
(
    FinanciamentoId INT IDENTITY(1,1) NOT NULL,
    ProjetoId       INT               NOT NULL,
    FonteId         INT               NOT NULL,
    Descricao       NVARCHAR(200)     NOT NULL,
    NumeroProcesso  NVARCHAR(80)      NULL,
    Valor           DECIMAL(18,2)     NOT NULL,
    DataInicio      DATE              NOT NULL,
    DataFim         DATE              NULL,
    Moeda           CHAR(3)           NOT NULL CONSTRAINT DF_Financiamento_Moeda DEFAULT ('BRL'),
    CriadoEm        DATETIME2(0)      NOT NULL CONSTRAINT DF_Financiamento_CriadoEm DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Financiamento PRIMARY KEY (FinanciamentoId),
    CONSTRAINT FK_Financiamento_Projeto FOREIGN KEY (ProjetoId) REFERENCES dbo.Projeto (ProjetoId),
    CONSTRAINT FK_Financiamento_Fonte FOREIGN KEY (FonteId) REFERENCES dbo.FonteFinanciadora (FonteId),
    CONSTRAINT CK_Financiamento_Valor CHECK (Valor > 0),
    CONSTRAINT CK_Financiamento_Moeda CHECK (Moeda = 'BRL'),
    CONSTRAINT CK_Financiamento_Datas CHECK (DataFim IS NULL OR DataFim >= DataInicio)
);
GO

CREATE INDEX IX_Financiamento_Projeto ON dbo.Financiamento (ProjetoId);
CREATE INDEX IX_Financiamento_Fonte ON dbo.Financiamento (FonteId);
CREATE UNIQUE INDEX UX_Financiamento_Processo
    ON dbo.Financiamento (ProjetoId, FonteId, NumeroProcesso)
    WHERE NumeroProcesso IS NOT NULL;
GO

/* ---------------------------------------------------------------------------
   9. Usuários, perfis e auditoria
   --------------------------------------------------------------------------- */
CREATE TABLE dbo.Usuario
(
    UsuarioId           INT IDENTITY(1,1) NOT NULL,
    Login               NVARCHAR(60)      NOT NULL,
    NomeCompleto        NVARCHAR(150)     NOT NULL,
    Email               NVARCHAR(150)     NOT NULL,
    SenhaHash           NVARCHAR(400)     NOT NULL,
    PerfilId            INT               NOT NULL,
    PesquisadorId       INT               NULL,
    Ativo               BIT               NOT NULL CONSTRAINT DF_Usuario_Ativo DEFAULT (1),
    UltimoAcesso        DATETIME2(0)      NULL,
    TentativasFalhas    INT               NOT NULL CONSTRAINT DF_Usuario_Tentativas DEFAULT (0),
    BloqueadoAte        DATETIME2(0)      NULL,
    CriadoEm            DATETIME2(0)      NOT NULL CONSTRAINT DF_Usuario_CriadoEm DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Usuario PRIMARY KEY (UsuarioId),
    CONSTRAINT FK_Usuario_Perfil FOREIGN KEY (PerfilId) REFERENCES dbo.PerfilAcesso (PerfilId),
    CONSTRAINT FK_Usuario_Pesquisador FOREIGN KEY (PesquisadorId) REFERENCES dbo.Pesquisador (PesquisadorId),
    CONSTRAINT UQ_Usuario_Login UNIQUE (Login),
    CONSTRAINT UQ_Usuario_Email UNIQUE (Email),
    CONSTRAINT CK_Usuario_Tentativas CHECK (TentativasFalhas >= 0)
);
GO

CREATE INDEX IX_Usuario_Perfil ON dbo.Usuario (PerfilId);
GO

CREATE TABLE dbo.LogAuditoria
(
    LogId       BIGINT IDENTITY(1,1) NOT NULL,
    UsuarioId   INT               NULL,
    Acao        NVARCHAR(30)      NOT NULL,
    Entidade    NVARCHAR(60)      NOT NULL,
    EntidadeId  INT               NULL,
    Detalhe     NVARCHAR(500)     NULL,
    DataHora    DATETIME2(0)      NOT NULL CONSTRAINT DF_Log_DataHora DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_LogAuditoria PRIMARY KEY (LogId),
    CONSTRAINT FK_Log_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuario (UsuarioId) ON DELETE SET NULL
);
GO

CREATE INDEX IX_Log_DataHora ON dbo.LogAuditoria (DataHora DESC);
GO

/* ---------------------------------------------------------------------------
   10. Triggers de integridade de negócio
   --------------------------------------------------------------------------- */

-- 10.1 Coerência da especialização (Pessoa x Pesquisador/Estudante)
CREATE TRIGGER dbo.TRG_Pesquisador_TipoPessoa
ON dbo.Pesquisador
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.Pessoa p ON p.PessoaId = i.PesquisadorId
        WHERE p.TipoPessoa <> 'P'
    )
    BEGIN
        ;THROW 50010, N'Um pesquisador só pode ser criado a partir de uma pessoa do tipo Pesquisador.', 1;
    END
END;
GO

CREATE TRIGGER dbo.TRG_Estudante_TipoPessoa
ON dbo.Estudante
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.Pessoa p ON p.PessoaId = i.EstudanteId
        WHERE p.TipoPessoa <> 'E'
    )
    BEGIN
        ;THROW 50011, N'Um estudante só pode ser criado a partir de uma pessoa do tipo Estudante.', 1;
    END
END;
GO

CREATE TRIGGER dbo.TRG_Pessoa_ProtegerTipo
ON dbo.Pessoa
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(TipoPessoa)
    BEGIN
        IF EXISTS (
            SELECT 1 FROM inserted i
            WHERE (i.TipoPessoa = 'P' AND EXISTS (SELECT 1 FROM dbo.Estudante e WHERE e.EstudanteId = i.PessoaId))
               OR (i.TipoPessoa = 'E' AND EXISTS (SELECT 1 FROM dbo.Pesquisador p WHERE p.PesquisadorId = i.PessoaId))
        )
        BEGIN
            ;THROW 50012, N'O tipo da pessoa não pode ser alterado enquanto existir vínculo como pesquisador ou estudante.', 1;
        END
    END
END;
GO

-- 10.2 Todo estudante ativo deve manter ao menos uma orientação principal ativa.
-- A troca de orientador ocorre em transação (encerra a anterior e cria a nova),
-- por isso o trigger atua em DELETE. O encerramento por UPDATE é validado pela
-- camada de negócio antes da atualização.
CREATE TRIGGER dbo.TRG_Orientacao_ProtegerPrincipal
ON dbo.Orientacao
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM deleted d
        JOIN dbo.Estudante e ON e.EstudanteId = d.EstudanteId
        WHERE d.Principal = 1
          AND d.DataFim IS NULL
          AND e.Situacao = N'Ativo'
          AND NOT EXISTS (
              SELECT 1 FROM dbo.Orientacao o
              WHERE o.EstudanteId = d.EstudanteId
                AND o.Principal = 1
                AND o.DataFim IS NULL
          )
    )
    BEGIN
        ;THROW 50020, N'Um estudante ativo não pode ficar sem orientador principal. Cadastre um novo orientador antes de encerrar a orientação atual.', 1;
    END
END;
GO

-- 10.3 Todo estudante ativo deve manter ao menos um projeto ativo (RN01)
CREATE TRIGGER dbo.TRG_EstudanteProjeto_ProtegerVinculo
ON dbo.EstudanteProjeto
AFTER DELETE, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM deleted d
        JOIN dbo.Estudante e ON e.EstudanteId = d.EstudanteId
        WHERE d.DataSaida IS NULL
          AND e.Situacao = N'Ativo'
          AND NOT EXISTS (
              SELECT 1 FROM dbo.EstudanteProjeto ep
              WHERE ep.EstudanteId = d.EstudanteId
                AND ep.DataSaida IS NULL
          )
    )
    BEGIN
        ;THROW 50021, N'Um estudante ativo não pode ficar sem vínculo com projeto. Associe-o a outro projeto antes de encerrar o vínculo atual.', 1;
    END
END;
GO

-- 10.4 Coordenador não pode ser duplicado na tabela de participação
CREATE TRIGGER dbo.TRG_Participacao_Validar
ON dbo.ParticipacaoProjeto
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN dbo.Projeto p ON p.ProjetoId = i.ProjetoId
        WHERE p.CoordenadorId = i.PesquisadorId
    )
    BEGIN
        ;THROW 50030, N'O coordenador já é participante implícito do projeto e não deve ser cadastrado novamente como participante.', 1;
    END
END;
GO

-- 10.5 Destaques: limite de 5 e vínculo com autoria
CREATE TRIGGER dbo.TRG_Destaque_Validar
ON dbo.DestaquePublicacao
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        WHERE NOT EXISTS (
            SELECT 1 FROM dbo.PublicacaoAutor pa
            WHERE pa.PublicacaoId = i.PublicacaoId
              AND pa.PesquisadorId = i.PesquisadorId
        )
    )
    BEGIN
        ;THROW 50040, N'Só é possível destacar uma publicação da qual o pesquisador seja autor.', 1;
    END

    IF EXISTS (
        SELECT d.PesquisadorId
        FROM dbo.DestaquePublicacao d
        WHERE d.PesquisadorId IN (SELECT i.PesquisadorId FROM inserted i)
        GROUP BY d.PesquisadorId
        HAVING COUNT(*) > 5
    )
    BEGIN
        ;THROW 50041, N'Cada pesquisador pode manter no máximo 5 publicações em destaque.', 1;
    END
END;
GO

/* A regra "publicação possui ao menos um autor" é garantida pela camada de
   negócio dentro de transação (incluir/atualizar publicação e autores juntos)
   e monitorada pela view vw_AlertasIntegridade. Um trigger de DELETE impediria
   a substituição legítima da lista de autores, por isso não é usado aqui. */

PRINT N'Estrutura do banco $(DbName) criada com sucesso.';
GO
