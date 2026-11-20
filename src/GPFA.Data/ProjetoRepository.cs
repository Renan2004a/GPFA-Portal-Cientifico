using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GPFA.Models;

namespace GPFA.Data
{
    public class ProjetoRepository
    {
        public List<ProjetoResumo> Listar(string termo = null, string situacao = null,
            int? coordenadorId = null, bool? destaque = null, string linha = null)
        {
            var sql = "SELECT * FROM dbo.vw_ProjetoResumo WHERE 1 = 1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                sql += @" AND (Titulo LIKE @Termo OR Sigla LIKE @Termo OR Resumo LIKE @Termo
                          OR LinhaPesquisa LIKE @Termo OR CoordenadorNome LIKE @Termo)";
                parameters.Add(SqlHelper.Param("@Termo", "%" + termo.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(situacao))
            {
                sql += " AND Situacao = @Situacao";
                parameters.Add(SqlHelper.Param("@Situacao", situacao));
            }
            if (coordenadorId.HasValue)
            {
                sql += " AND CoordenadorId = @Coordenador";
                parameters.Add(SqlHelper.Param("@Coordenador", coordenadorId.Value));
            }
            if (destaque.HasValue)
            {
                sql += " AND Destaque = @Destaque";
                parameters.Add(SqlHelper.Param("@Destaque", destaque.Value));
            }
            if (!string.IsNullOrWhiteSpace(linha))
            {
                sql += " AND LinhaPesquisa = @Linha";
                parameters.Add(SqlHelper.Param("@Linha", linha.Trim()));
            }

            sql += @" ORDER BY CASE WHEN Situacao = N'Em andamento' THEN 0
                                   WHEN Situacao = N'Planejamento' THEN 1 ELSE 2 END,
                              DataInicio DESC";
            return SqlHelper.Query(sql, Mappers.ProjetoResumo, parameters.ToArray());
        }

        public ProjetoResumo ObterResumo(int projetoId)
        {
            return SqlHelper.QuerySingle("SELECT * FROM dbo.vw_ProjetoResumo WHERE ProjetoId = @Id",
                Mappers.ProjetoResumo, SqlHelper.Param("@Id", projetoId));
        }

        public Projeto ObterEntidade(int projetoId)
        {
            const string sql = @"
SELECT ProjetoId, Titulo, Sigla, Resumo, Descricao, LinhaPesquisa, Situacao,
       DataInicio, DataFimPrevista, DataFim, CoordenadorId, ImagemUrl, Destaque, CriadoEm, AtualizadoEm
FROM dbo.Projeto WHERE ProjetoId = @Id";
            return SqlHelper.QuerySingle(sql, r => new Projeto
            {
                ProjetoId = DbValue.Int(r, "ProjetoId"),
                Titulo = DbValue.String(r, "Titulo"),
                Sigla = DbValue.String(r, "Sigla"),
                Resumo = DbValue.String(r, "Resumo"),
                Descricao = DbValue.String(r, "Descricao"),
                LinhaPesquisa = DbValue.String(r, "LinhaPesquisa"),
                Situacao = DbValue.String(r, "Situacao"),
                DataInicio = DbValue.Date(r, "DataInicio"),
                DataFimPrevista = DbValue.DateNull(r, "DataFimPrevista"),
                DataFim = DbValue.DateNull(r, "DataFim"),
                CoordenadorId = DbValue.Int(r, "CoordenadorId"),
                ImagemUrl = DbValue.String(r, "ImagemUrl"),
                Destaque = DbValue.Bool(r, "Destaque"),
                CriadoEm = DbValue.Date(r, "CriadoEm"),
                AtualizadoEm = DbValue.Date(r, "AtualizadoEm")
            }, SqlHelper.Param("@Id", projetoId));
        }

        public int Inserir(Projeto projeto)
        {
            return Convert.ToInt32(SqlHelper.Scalar(@"
INSERT dbo.Projeto (Titulo, Sigla, Resumo, Descricao, LinhaPesquisa, Situacao,
                    DataInicio, DataFimPrevista, DataFim, CoordenadorId, ImagemUrl, Destaque)
VALUES (@Titulo, @Sigla, @Resumo, @Descricao, @Linha, @Situacao,
        @Inicio, @FimPrevisto, @Fim, @Coordenador, @Imagem, @Destaque);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                SqlHelper.Param("@Titulo", projeto.Titulo.Trim()),
                SqlHelper.Param("@Sigla", DbValue.OrDBNull(projeto.Sigla)),
                SqlHelper.Param("@Resumo", projeto.Resumo.Trim()),
                SqlHelper.Param("@Descricao", DbValue.OrDBNull(projeto.Descricao)),
                SqlHelper.Param("@Linha", DbValue.OrDBNull(projeto.LinhaPesquisa)),
                SqlHelper.Param("@Situacao", projeto.Situacao),
                SqlHelper.Param("@Inicio", projeto.DataInicio),
                SqlHelper.Param("@FimPrevisto", DbValue.OrDBNull(projeto.DataFimPrevista)),
                SqlHelper.Param("@Fim", DbValue.OrDBNull(projeto.DataFim)),
                SqlHelper.Param("@Coordenador", projeto.CoordenadorId),
                SqlHelper.Param("@Imagem", DbValue.OrDBNull(projeto.ImagemUrl)),
                SqlHelper.Param("@Destaque", projeto.Destaque)));
        }

        public void Atualizar(Projeto projeto)
        {
            SqlHelper.Execute(@"
UPDATE dbo.Projeto
SET Titulo = @Titulo, Sigla = @Sigla, Resumo = @Resumo, Descricao = @Descricao,
    LinhaPesquisa = @Linha, Situacao = @Situacao, DataInicio = @Inicio,
    DataFimPrevista = @FimPrevisto, DataFim = @Fim, CoordenadorId = @Coordenador,
    ImagemUrl = @Imagem, Destaque = @Destaque, AtualizadoEm = SYSDATETIME()
WHERE ProjetoId = @Id",
                SqlHelper.Param("@Titulo", projeto.Titulo.Trim()),
                SqlHelper.Param("@Sigla", DbValue.OrDBNull(projeto.Sigla)),
                SqlHelper.Param("@Resumo", projeto.Resumo.Trim()),
                SqlHelper.Param("@Descricao", DbValue.OrDBNull(projeto.Descricao)),
                SqlHelper.Param("@Linha", DbValue.OrDBNull(projeto.LinhaPesquisa)),
                SqlHelper.Param("@Situacao", projeto.Situacao),
                SqlHelper.Param("@Inicio", projeto.DataInicio),
                SqlHelper.Param("@FimPrevisto", DbValue.OrDBNull(projeto.DataFimPrevista)),
                SqlHelper.Param("@Fim", DbValue.OrDBNull(projeto.DataFim)),
                SqlHelper.Param("@Coordenador", projeto.CoordenadorId),
                SqlHelper.Param("@Imagem", DbValue.OrDBNull(projeto.ImagemUrl)),
                SqlHelper.Param("@Destaque", projeto.Destaque),
                SqlHelper.Param("@Id", projeto.ProjetoId));
        }

        public void Excluir(int projetoId)
        {
            SqlHelper.Execute("DELETE dbo.Projeto WHERE ProjetoId = @Id",
                SqlHelper.Param("@Id", projetoId));
        }

        public List<ProjetoResumo> ListarDestaques(int limite)
        {
            return SqlHelper.Query(@"
SELECT TOP (@Limite) * FROM dbo.vw_ProjetoResumo
ORDER BY Destaque DESC, CASE WHEN Situacao = N'Em andamento' THEN 0 ELSE 1 END, DataInicio DESC",
                Mappers.ProjetoResumo, SqlHelper.Param("@Limite", limite));
        }

        public List<ProjetoResumo> ListarDoCoordenador(int pesquisadorId)
        {
            return SqlHelper.Query(
                "SELECT * FROM dbo.vw_ProjetoResumo WHERE CoordenadorId = @Id ORDER BY DataInicio DESC",
                Mappers.ProjetoResumo, SqlHelper.Param("@Id", pesquisadorId));
        }

        public List<OpcaoSelecao> ListarOpcoes()
        {
            return SqlHelper.Query(@"
SELECT ProjetoId AS Id,
       CASE WHEN Sigla IS NULL THEN Titulo ELSE Sigla + N' — ' + Titulo END AS Texto
FROM dbo.Projeto ORDER BY Titulo", r => new OpcaoSelecao
            {
                Id = DbValue.Int(r, "Id"),
                Texto = DbValue.String(r, "Texto")
            });
        }

        /* ----------------------------- Participações ----------------------------- */

        public List<ParticipacaoProjeto> ListarParticipacoes(int projetoId)
        {
            return SqlHelper.Query(@"
SELECT pp.ParticipacaoId, pp.ProjetoId, pp.PesquisadorId,
       p.NomeCompleto AS PesquisadorNome, p.FotoUrl AS PesquisadorFotoUrl, pp.Papel, pp.DataEntrada, pp.DataSaida
FROM dbo.ParticipacaoProjeto pp
JOIN dbo.Pessoa p ON p.PessoaId = pp.PesquisadorId
WHERE pp.ProjetoId = @Id
ORDER BY pp.DataSaida, p.NomeCompleto", Mappers.Participacao,
                SqlHelper.Param("@Id", projetoId));
        }

        public List<PesquisadorResumo> ListarParticipantesAtivos(int projetoId)
        {
            return SqlHelper.Query(@"
SELECT p.*
FROM dbo.vw_PesquisadorResumo p
JOIN dbo.ParticipacaoProjeto pp ON pp.PesquisadorId = p.PesquisadorId
WHERE pp.ProjetoId = @Id AND pp.DataSaida IS NULL
ORDER BY p.NomeCompleto", Mappers.PesquisadorResumo, SqlHelper.Param("@Id", projetoId));
        }

        public int InserirParticipacao(ParticipacaoProjeto participacao)
        {
            return Convert.ToInt32(SqlHelper.Scalar(@"
INSERT dbo.ParticipacaoProjeto (ProjetoId, PesquisadorId, Papel, DataEntrada, DataSaida)
VALUES (@Projeto, @Pesquisador, @Papel, @Entrada, NULL);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                SqlHelper.Param("@Projeto", participacao.ProjetoId),
                SqlHelper.Param("@Pesquisador", participacao.PesquisadorId),
                SqlHelper.Param("@Papel", participacao.Papel),
                SqlHelper.Param("@Entrada", participacao.DataEntrada)));
        }

        public void EncerrarParticipacao(int participacaoId, DateTime dataSaida)
        {
            SqlHelper.Execute(@"
UPDATE dbo.ParticipacaoProjeto SET DataSaida = @Saida
WHERE ParticipacaoId = @Id AND DataSaida IS NULL;",
                SqlHelper.Param("@Saida", dataSaida),
                SqlHelper.Param("@Id", participacaoId));
        }

        public void ExcluirParticipacao(int participacaoId)
        {
            SqlHelper.Execute("DELETE dbo.ParticipacaoProjeto WHERE ParticipacaoId = @Id",
                SqlHelper.Param("@Id", participacaoId));
        }

        public ParticipacaoProjeto ObterParticipacao(int participacaoId)
        {
            return SqlHelper.QuerySingle(@"
SELECT pp.ParticipacaoId, pp.ProjetoId, pp.PesquisadorId,
       p.NomeCompleto AS PesquisadorNome, p.FotoUrl AS PesquisadorFotoUrl, pp.Papel, pp.DataEntrada, pp.DataSaida
FROM dbo.ParticipacaoProjeto pp
JOIN dbo.Pessoa p ON p.PessoaId = pp.PesquisadorId
WHERE pp.ParticipacaoId = @Id", Mappers.Participacao,
                SqlHelper.Param("@Id", participacaoId));
        }

        /* ------------------------- Estudantes no projeto ------------------------- */

        public List<EstudanteResumo> ListarEstudantes(int projetoId)
        {
            return SqlHelper.Query(@"
SELECT e.*
FROM dbo.vw_EstudanteResumo e
JOIN dbo.EstudanteProjeto ep ON ep.EstudanteId = e.EstudanteId
WHERE ep.ProjetoId = @Id
ORDER BY CASE WHEN ep.DataSaida IS NULL THEN 0 ELSE 1 END, e.NomeCompleto",
                Mappers.EstudanteResumo, SqlHelper.Param("@Id", projetoId));
        }

        public List<EstudanteProjeto> ListarVinculosEstudantes(int projetoId)
        {
            return SqlHelper.Query(@"
SELECT ep.EstudanteProjetoId, ep.EstudanteId, ep.ProjetoId,
       p.NomeCompleto AS EstudanteNome, pr.Titulo AS ProjetoTitulo,
       ep.DataEntrada, ep.DataSaida, ep.TipoVinculo
FROM dbo.EstudanteProjeto ep
JOIN dbo.Pessoa p ON p.PessoaId = ep.EstudanteId
JOIN dbo.Projeto pr ON pr.ProjetoId = ep.ProjetoId
WHERE ep.ProjetoId = @Id
ORDER BY ep.DataSaida, p.NomeCompleto", r => new EstudanteProjeto
            {
                EstudanteProjetoId = DbValue.Int(r, "EstudanteProjetoId"),
                EstudanteId = DbValue.Int(r, "EstudanteId"),
                ProjetoId = DbValue.Int(r, "ProjetoId"),
                EstudanteNome = DbValue.String(r, "EstudanteNome"),
                ProjetoTitulo = DbValue.String(r, "ProjetoTitulo"),
                DataEntrada = DbValue.Date(r, "DataEntrada"),
                DataSaida = DbValue.DateNull(r, "DataSaida"),
                TipoVinculo = DbValue.String(r, "TipoVinculo")
            }, SqlHelper.Param("@Id", projetoId));
        }

        public void VincularEstudante(int projetoId, int estudanteId, DateTime dataEntrada, string tipoVinculo)
        {
            SqlHelper.Execute(@"
INSERT dbo.EstudanteProjeto (EstudanteId, ProjetoId, DataEntrada, DataSaida, TipoVinculo)
VALUES (@Estudante, @Projeto, @Entrada, NULL, @Tipo);",
                SqlHelper.Param("@Estudante", estudanteId),
                SqlHelper.Param("@Projeto", projetoId),
                SqlHelper.Param("@Entrada", dataEntrada),
                SqlHelper.Param("@Tipo", DbValue.OrDBNull(tipoVinculo)));
        }

        public void EncerrarVinculoEstudante(int estudanteProjetoId, DateTime dataSaida)
        {
            SqlHelper.Execute(@"
UPDATE dbo.EstudanteProjeto SET DataSaida = @Saida
WHERE EstudanteProjetoId = @Id AND DataSaida IS NULL;",
                SqlHelper.Param("@Saida", dataSaida),
                SqlHelper.Param("@Id", estudanteProjetoId));
        }

        public void ExcluirVinculoEstudante(int estudanteProjetoId)
        {
            SqlHelper.Execute("DELETE dbo.EstudanteProjeto WHERE EstudanteProjetoId = @Id",
                SqlHelper.Param("@Id", estudanteProjetoId));
        }

        public int ContarProjetosAtivosDoEstudante(int estudanteId)
        {
            return SqlHelper.ScalarInt(@"
SELECT COUNT(*) FROM dbo.EstudanteProjeto
WHERE EstudanteId = @Id AND DataSaida IS NULL",
                SqlHelper.Param("@Id", estudanteId));
        }

        public VinculosProjeto ObterVinculos(int projetoId)
        {
            return SqlHelper.QuerySingle(@"
SELECT
  (SELECT COUNT(*) FROM dbo.Publicacao WHERE ProjetoId = @Id)           AS Publicacoes,
  (SELECT COUNT(*) FROM dbo.Financiamento WHERE ProjetoId = @Id)        AS Financiamentos,
  (SELECT COUNT(*) FROM dbo.ParticipacaoProjeto WHERE ProjetoId = @Id)  AS Participacoes,
  (SELECT COUNT(*) FROM dbo.EstudanteProjeto WHERE ProjetoId = @Id)     AS Estudantes",
                r => new VinculosProjeto
                {
                    Publicacoes = DbValue.Int(r, "Publicacoes"),
                    Financiamentos = DbValue.Int(r, "Financiamentos"),
                    Participacoes = DbValue.Int(r, "Participacoes"),
                    Estudantes = DbValue.Int(r, "Estudantes")
                }, SqlHelper.Param("@Id", projetoId));
        }

        public int ObterProjetoIdDoVinculo(int estudanteProjetoId)
        {
            return SqlHelper.ScalarInt(
                "SELECT ProjetoId FROM dbo.EstudanteProjeto WHERE EstudanteProjetoId = @Id",
                SqlHelper.Param("@Id", estudanteProjetoId));
        }
    }
}
