/* ============================================================================
   GPFA - Script 02 - Visões de consulta e relatórios
   Todas as somas financeiras são calculadas por projeto (uma única vez por
   registro de financiamento), evitando duplicidade quando o pesquisador
   participa de mais de um projeto ou quando há vários participantes.
   ============================================================================ */
:setvar DbName "GPFA_Portal"
USE [$(DbName)];
GO

/* ---------------------------------------------------------------------------
   Projetos: dados consolidados para cards, páginas e relatórios
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_ProjetoResumo
AS
SELECT
    pr.ProjetoId,
    pr.Titulo,
    pr.Sigla,
    pr.Resumo,
    pr.Descricao,
    pr.LinhaPesquisa,
    pr.Situacao,
    pr.DataInicio,
    pr.DataFimPrevista,
    pr.DataFim,
    pr.CoordenadorId,
    pes.NomeCompleto                       AS CoordenadorNome,
    pes.FotoUrl                            AS CoordenadorFotoUrl,
    pr.ImagemUrl,
    pr.Destaque,
    pr.CriadoEm,
    pr.AtualizadoEm,
    1 + ISNULL((
        SELECT COUNT(*) FROM dbo.ParticipacaoProjeto pp
        WHERE pp.ProjetoId = pr.ProjetoId AND pp.DataSaida IS NULL
    ), 0)                                  AS QtdPesquisadores,
    ISNULL((
        SELECT COUNT(*) FROM dbo.EstudanteProjeto ep
        WHERE ep.ProjetoId = pr.ProjetoId AND ep.DataSaida IS NULL
    ), 0)                                  AS QtdEstudantes,
    ISNULL((
        SELECT COUNT(*) FROM dbo.Publicacao pu
        WHERE pu.ProjetoId = pr.ProjetoId
    ), 0)                                  AS QtdPublicacoes,
    ISNULL((
        SELECT SUM(f.Valor) FROM dbo.Financiamento f
        WHERE f.ProjetoId = pr.ProjetoId
    ), 0)                                  AS TotalCaptado
FROM dbo.Projeto pr
JOIN dbo.Pesquisador p   ON p.PesquisadorId = pr.CoordenadorId
JOIN dbo.Pessoa pes      ON pes.PessoaId = pr.CoordenadorId;
GO

/* ---------------------------------------------------------------------------
   Pesquisadores: dados consolidados do perfil
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_PesquisadorResumo
AS
SELECT
    p.PesquisadorId,
    pe.NomeCompleto,
    pe.Email,
    pe.FotoUrl,
    pe.FormacaoAcademica,
    pe.AreaAtuacao,
    pe.MiniBiografia,
    pe.GoogleScholarUrl,
    pe.OrcidUrl,
    pe.LattesUrl,
    pe.Ativo,
    p.Titulacao,
    p.Instituicao,
    p.Departamento,
    p.LinhaPesquisa,
    ISNULL((
        SELECT COUNT(*) FROM dbo.Projeto pr
        WHERE pr.CoordenadorId = p.PesquisadorId
    ), 0)                                  AS QtdProjetosCoordenados,
    ISNULL((
        SELECT COUNT(DISTINCT pr.ProjetoId)
        FROM dbo.Projeto pr
        WHERE pr.CoordenadorId = p.PesquisadorId
           OR EXISTS (SELECT 1 FROM dbo.ParticipacaoProjeto pp
                      WHERE pp.ProjetoId = pr.ProjetoId AND pp.PesquisadorId = p.PesquisadorId)
    ), 0)                                  AS QtdProjetos,
    ISNULL((
        SELECT COUNT(*) FROM dbo.Orientacao o
        JOIN dbo.Estudante e ON e.EstudanteId = o.EstudanteId
        WHERE o.OrientadorId = p.PesquisadorId AND o.DataFim IS NULL AND e.Situacao = N'Ativo'
    ), 0)                                  AS QtdOrientandosAtuais,
    ISNULL((
        SELECT COUNT(*) FROM dbo.Orientacao o
        WHERE o.OrientadorId = p.PesquisadorId AND o.DataFim IS NOT NULL
    ), 0)                                  AS QtdOrientandosPassados,
    ISNULL((
        SELECT COUNT(*) FROM dbo.PublicacaoAutor pa
        WHERE pa.PesquisadorId = p.PesquisadorId
    ), 0)                                  AS QtdPublicacoes,
    ISNULL((
        SELECT SUM(x.TotalProjeto)
        FROM (
            SELECT pr.ProjetoId, SUM(f.Valor) AS TotalProjeto
            FROM dbo.Projeto pr
            JOIN dbo.Financiamento f ON f.ProjetoId = pr.ProjetoId
            WHERE pr.CoordenadorId = p.PesquisadorId
               OR EXISTS (SELECT 1 FROM dbo.ParticipacaoProjeto pp
                          WHERE pp.ProjetoId = pr.ProjetoId AND pp.PesquisadorId = p.PesquisadorId)
            GROUP BY pr.ProjetoId
        ) x
    ), 0)                                  AS TotalCaptado
FROM dbo.Pesquisador p
JOIN dbo.Pessoa pe ON pe.PessoaId = p.PesquisadorId;
GO

/* ---------------------------------------------------------------------------
   Projetos por pesquisador (coordenação ou participação)
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_PesquisadorProjeto
AS
SELECT
    p.PesquisadorId,
    pe.NomeCompleto,
    pr.ProjetoId,
    pr.Titulo,
    pr.Situacao,
    pr.DataInicio,
    pr.DataFim,
    CASE WHEN pr.CoordenadorId = p.PesquisadorId THEN N'Coordenador' ELSE N'Participante' END AS Papel,
    pp.Papel                            AS PapelParticipacao,
    ISNULL(f.Total, 0)                  AS TotalCaptadoProjeto
FROM dbo.Pesquisador p
JOIN dbo.Pessoa pe ON pe.PessoaId = p.PesquisadorId
JOIN dbo.Projeto pr ON pr.CoordenadorId = p.PesquisadorId
    OR EXISTS (SELECT 1 FROM dbo.ParticipacaoProjeto x
               WHERE x.ProjetoId = pr.ProjetoId AND x.PesquisadorId = p.PesquisadorId)
LEFT JOIN dbo.ParticipacaoProjeto pp
       ON pp.ProjetoId = pr.ProjetoId AND pp.PesquisadorId = p.PesquisadorId
OUTER APPLY (
    SELECT SUM(fi.Valor) AS Total FROM dbo.Financiamento fi WHERE fi.ProjetoId = pr.ProjetoId
) f;
GO

/* ---------------------------------------------------------------------------
   Orientandos atuais e passados (RN02)
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_Orientandos
AS
SELECT
    o.OrientacaoId,
    o.OrientadorId,
    peOri.NomeCompleto                  AS OrientadorNome,
    e.EstudanteId,
    peEst.NomeCompleto                  AS EstudanteNome,
    peEst.Email                         AS EstudanteEmail,
    peEst.FotoUrl                       AS EstudanteFotoUrl,
    e.NivelAcademico,
    e.Curso,
    e.Situacao,
    e.CoorientadoresExternos,
    o.Principal,
    o.DataInicio,
    o.DataFim,
    CASE WHEN o.DataFim IS NULL AND e.Situacao = N'Ativo' THEN N'Atual' ELSE N'Passado' END AS Vinculo
FROM dbo.Orientacao o
JOIN dbo.Pesquisador po ON po.PesquisadorId = o.OrientadorId
JOIN dbo.Pessoa peOri   ON peOri.PessoaId = o.OrientadorId
JOIN dbo.Estudante e    ON e.EstudanteId = o.EstudanteId
JOIN dbo.Pessoa peEst   ON peEst.PessoaId = e.EstudanteId;
GO

/* ---------------------------------------------------------------------------
   Estudantes: diretório com orientador principal e projetos
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_EstudanteResumo
AS
SELECT
    e.EstudanteId,
    pe.NomeCompleto,
    pe.Email,
    pe.FotoUrl,
    pe.FormacaoAcademica,
    pe.AreaAtuacao,
    e.NivelAcademico,
    e.Curso,
    e.Situacao,
    e.DataIngresso,
    e.DataConclusao,
    e.CoorientadoresExternos,
    o.OrientadorId,
    peOri.NomeCompleto                  AS OrientadorNome,
    ISNULL((
        SELECT COUNT(*) FROM dbo.EstudanteProjeto ep
        WHERE ep.EstudanteId = e.EstudanteId AND ep.DataSaida IS NULL
    ), 0)                               AS QtdProjetosAtivos,
    ISNULL((
        SELECT STRING_AGG(pr.Sigla, N' · ') WITHIN GROUP (ORDER BY pr.Titulo)
        FROM dbo.EstudanteProjeto ep
        JOIN dbo.Projeto pr ON pr.ProjetoId = ep.ProjetoId
        WHERE ep.EstudanteId = e.EstudanteId AND ep.DataSaida IS NULL
    ), N'')                             AS ProjetosAtivos
FROM dbo.Estudante e
JOIN dbo.Pessoa pe ON pe.PessoaId = e.EstudanteId
LEFT JOIN dbo.Orientacao o ON o.EstudanteId = e.EstudanteId AND o.Principal = 1 AND o.DataFim IS NULL
LEFT JOIN dbo.Pessoa peOri ON peOri.PessoaId = o.OrientadorId;
GO

/* ---------------------------------------------------------------------------
   Publicações: detalhe com autores concatenados
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_PublicacaoDetalhe
AS
SELECT
    pu.PublicacaoId,
    pu.Titulo,
    pu.Veiculo,
    pu.Ano,
    pu.Tipo,
    pu.Doi,
    pu.LinkAcesso,
    pu.ProjetoId,
    pr.Titulo                           AS ProjetoTitulo,
    pr.Sigla                            AS ProjetoSigla,
    pu.CriadoEm,
    STUFF((
        SELECT N'; ' + pe.NomeCompleto
        FROM dbo.PublicacaoAutor pa
        JOIN dbo.Pessoa pe ON pe.PessoaId = pa.PesquisadorId
        WHERE pa.PublicacaoId = pu.PublicacaoId
        ORDER BY pa.OrdemAutoria
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, N'') AS Autores
FROM dbo.Publicacao pu
LEFT JOIN dbo.Projeto pr ON pr.ProjetoId = pu.ProjetoId;
GO

/* ---------------------------------------------------------------------------
   Financiamentos por projeto e por fonte
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_FinanciamentoPorProjeto
AS
SELECT
    pr.ProjetoId,
    pr.Titulo,
    pr.Situacao,
    COUNT(f.FinanciamentoId)            AS QtdFinanciamentos,
    ISNULL(SUM(f.Valor), 0)             AS TotalCaptado,
    MIN(f.DataInicio)                   AS PrimeiroFinanciamento,
    MAX(f.DataFim)                      AS UltimoEncerramento
FROM dbo.Projeto pr
LEFT JOIN dbo.Financiamento f ON f.ProjetoId = pr.ProjetoId
GROUP BY pr.ProjetoId, pr.Titulo, pr.Situacao;
GO

CREATE VIEW dbo.vw_FinanciamentoPorFonte
AS
SELECT
    fo.FonteId,
    fo.Nome,
    fo.Sigla,
    fo.Tipo,
    fo.SiteUrl,
    COUNT(f.FinanciamentoId)            AS QtdFinanciamentos,
    COUNT(DISTINCT f.ProjetoId)         AS QtdProjetos,
    ISNULL(SUM(f.Valor), 0)             AS TotalCaptado
FROM dbo.FonteFinanciadora fo
LEFT JOIN dbo.Financiamento f ON f.FonteId = fo.FonteId
GROUP BY fo.FonteId, fo.Nome, fo.Sigla, fo.Tipo, fo.SiteUrl;
GO

/* ---------------------------------------------------------------------------
   Financiamento por pesquisador (detalhado por projeto, sem duplicidade)
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_FinanciamentoPorPesquisador
AS
SELECT
    p.PesquisadorId,
    pe.NomeCompleto,
    pr.ProjetoId,
    pr.Titulo                           AS ProjetoTitulo,
    CASE WHEN pr.CoordenadorId = p.PesquisadorId THEN N'Coordenador' ELSE N'Participante' END AS Papel,
    ISNULL(SUM(f.Valor), 0)             AS TotalCaptado
FROM dbo.Pesquisador p
JOIN dbo.Pessoa pe ON pe.PessoaId = p.PesquisadorId
JOIN dbo.Projeto pr ON pr.CoordenadorId = p.PesquisadorId
    OR EXISTS (SELECT 1 FROM dbo.ParticipacaoProjeto pp
               WHERE pp.ProjetoId = pr.ProjetoId AND pp.PesquisadorId = p.PesquisadorId)
LEFT JOIN dbo.Financiamento f ON f.ProjetoId = pr.ProjetoId
GROUP BY p.PesquisadorId, pe.NomeCompleto, pr.ProjetoId, pr.Titulo, pr.CoordenadorId;
GO

/* ---------------------------------------------------------------------------
   Indicadores e relatórios agregados
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_IndicadoresGerais
AS
SELECT
    (SELECT COUNT(*) FROM dbo.Pesquisador)                                      AS TotalPesquisadores,
    (SELECT COUNT(*) FROM dbo.Estudante)                                        AS TotalEstudantes,
    (SELECT COUNT(*) FROM dbo.Estudante WHERE Situacao = N'Ativo')              AS TotalEstudantesAtivos,
    (SELECT COUNT(*) FROM dbo.Projeto)                                          AS TotalProjetos,
    (SELECT COUNT(*) FROM dbo.Projeto WHERE Situacao = N'Em andamento')         AS TotalProjetosEmAndamento,
    (SELECT COUNT(*) FROM dbo.Publicacao)                                       AS TotalPublicacoes,
    (SELECT COUNT(*) FROM dbo.Financiamento)                                    AS TotalFinanciamentos,
    ISNULL((SELECT SUM(Valor) FROM dbo.Financiamento), 0)                       AS TotalCaptado,
    (SELECT COUNT(*) FROM dbo.FonteFinanciadora)                                AS TotalFontes;
GO

CREATE VIEW dbo.vw_ProjetosPorSituacao
AS
SELECT Situacao, COUNT(*) AS Quantidade
FROM dbo.Projeto
GROUP BY Situacao;
GO

CREATE VIEW dbo.vw_PublicacoesPorAno
AS
SELECT Ano, COUNT(*) AS Quantidade
FROM dbo.Publicacao
GROUP BY Ano;
GO

CREATE VIEW dbo.vw_PublicacoesPorPesquisador
AS
SELECT
    p.PesquisadorId,
    pe.NomeCompleto,
    COUNT(pa.PublicacaoId)              AS Quantidade,
    MAX(pu.Ano)                         AS UltimoAno
FROM dbo.Pesquisador p
JOIN dbo.Pessoa pe ON pe.PessoaId = p.PesquisadorId
LEFT JOIN dbo.PublicacaoAutor pa ON pa.PesquisadorId = p.PesquisadorId
LEFT JOIN dbo.Publicacao pu ON pu.PublicacaoId = pa.PublicacaoId
GROUP BY p.PesquisadorId, pe.NomeCompleto;
GO

CREATE VIEW dbo.vw_OrientandosPorOrientador
AS
SELECT
    o.OrientadorId,
    pe.NomeCompleto                     AS OrientadorNome,
    SUM(CASE WHEN o.DataFim IS NULL AND e.Situacao = N'Ativo' THEN 1 ELSE 0 END) AS Atuais,
    SUM(CASE WHEN o.DataFim IS NOT NULL OR e.Situacao <> N'Ativo' THEN 1 ELSE 0 END) AS Passados
FROM dbo.Orientacao o
JOIN dbo.Pesquisador p ON p.PesquisadorId = o.OrientadorId
JOIN dbo.Pessoa pe ON pe.PessoaId = o.OrientadorId
JOIN dbo.Estudante e ON e.EstudanteId = o.EstudanteId
GROUP BY o.OrientadorId, pe.NomeCompleto;
GO

/* ---------------------------------------------------------------------------
   Painel de integridade: detecta inconsistências que burlariam a aplicação
   --------------------------------------------------------------------------- */
CREATE VIEW dbo.vw_AlertasIntegridade
AS
SELECT N'Estudante ativo sem orientador principal' AS Alerta, e.EstudanteId AS RegistroId, pe.NomeCompleto AS Detalhe
FROM dbo.Estudante e
JOIN dbo.Pessoa pe ON pe.PessoaId = e.EstudanteId
WHERE e.Situacao = N'Ativo'
  AND NOT EXISTS (SELECT 1 FROM dbo.Orientacao o
                  WHERE o.EstudanteId = e.EstudanteId AND o.Principal = 1 AND o.DataFim IS NULL)
UNION ALL
SELECT N'Estudante ativo sem projeto', e.EstudanteId, pe.NomeCompleto
FROM dbo.Estudante e
JOIN dbo.Pessoa pe ON pe.PessoaId = e.EstudanteId
WHERE e.Situacao = N'Ativo'
  AND NOT EXISTS (SELECT 1 FROM dbo.EstudanteProjeto ep
                  WHERE ep.EstudanteId = e.EstudanteId AND ep.DataSaida IS NULL)
UNION ALL
SELECT N'Publicação sem autor', pu.PublicacaoId, pu.Titulo
FROM dbo.Publicacao pu
WHERE NOT EXISTS (SELECT 1 FROM dbo.PublicacaoAutor pa WHERE pa.PublicacaoId = pu.PublicacaoId);
GO

PRINT N'Visões criadas com sucesso.';
GO
