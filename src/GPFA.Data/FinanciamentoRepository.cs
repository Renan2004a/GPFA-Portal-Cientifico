using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GPFA.Models;

namespace GPFA.Data
{
    public class FinanciamentoRepository
    {
        private const string SelectBase = @"
SELECT f.FinanciamentoId, f.ProjetoId, pr.Titulo AS ProjetoTitulo, pr.Sigla AS ProjetoSigla,
       f.FonteId, fo.Nome AS FonteNome, fo.Sigla AS FonteSigla,
       f.Descricao, f.NumeroProcesso, f.Valor, f.DataInicio, f.DataFim, f.Moeda, f.CriadoEm
FROM dbo.Financiamento f
JOIN dbo.Projeto pr ON pr.ProjetoId = f.ProjetoId
JOIN dbo.FonteFinanciadora fo ON fo.FonteId = f.FonteId";

        public List<Financiamento> Listar(int? projetoId = null, int? fonteId = null, string termo = null)
        {
            var sql = SelectBase + " WHERE 1 = 1";
            var parameters = new List<SqlParameter>();

            if (projetoId.HasValue)
            {
                sql += " AND f.ProjetoId = @Projeto";
                parameters.Add(SqlHelper.Param("@Projeto", projetoId.Value));
            }
            if (fonteId.HasValue)
            {
                sql += " AND f.FonteId = @Fonte";
                parameters.Add(SqlHelper.Param("@Fonte", fonteId.Value));
            }
            if (!string.IsNullOrWhiteSpace(termo))
            {
                sql += " AND (pr.Titulo LIKE @Termo OR fo.Nome LIKE @Termo OR f.Descricao LIKE @Termo OR f.NumeroProcesso LIKE @Termo)";
                parameters.Add(SqlHelper.Param("@Termo", "%" + termo.Trim() + "%"));
            }

            sql += " ORDER BY f.DataInicio DESC, f.FinanciamentoId DESC";
            return SqlHelper.Query(sql, Mappers.FinanciamentoEntity, parameters.ToArray());
        }

        public Financiamento Obter(int financiamentoId)
        {
            return SqlHelper.QuerySingle(SelectBase + " WHERE f.FinanciamentoId = @Id",
                Mappers.FinanciamentoEntity, SqlHelper.Param("@Id", financiamentoId));
        }

        public int Inserir(Financiamento financiamento)
        {
            return Convert.ToInt32(SqlHelper.Scalar(@"
INSERT dbo.Financiamento (ProjetoId, FonteId, Descricao, NumeroProcesso, Valor, DataInicio, DataFim, Moeda)
VALUES (@Projeto, @Fonte, @Descricao, @Processo, @Valor, @Inicio, @Fim, 'BRL');
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                SqlHelper.Param("@Projeto", financiamento.ProjetoId),
                SqlHelper.Param("@Fonte", financiamento.FonteId),
                SqlHelper.Param("@Descricao", financiamento.Descricao.Trim()),
                SqlHelper.Param("@Processo", DbValue.OrDBNull(financiamento.NumeroProcesso)),
                SqlHelper.Param("@Valor", financiamento.Valor),
                SqlHelper.Param("@Inicio", financiamento.DataInicio),
                SqlHelper.Param("@Fim", DbValue.OrDBNull(financiamento.DataFim))));
        }

        public void Atualizar(Financiamento financiamento)
        {
            SqlHelper.Execute(@"
UPDATE dbo.Financiamento
SET ProjetoId = @Projeto, FonteId = @Fonte, Descricao = @Descricao,
    NumeroProcesso = @Processo, Valor = @Valor, DataInicio = @Inicio,
    DataFim = @Fim, Moeda = 'BRL'
WHERE FinanciamentoId = @Id",
                SqlHelper.Param("@Projeto", financiamento.ProjetoId),
                SqlHelper.Param("@Fonte", financiamento.FonteId),
                SqlHelper.Param("@Descricao", financiamento.Descricao.Trim()),
                SqlHelper.Param("@Processo", DbValue.OrDBNull(financiamento.NumeroProcesso)),
                SqlHelper.Param("@Valor", financiamento.Valor),
                SqlHelper.Param("@Inicio", financiamento.DataInicio),
                SqlHelper.Param("@Fim", DbValue.OrDBNull(financiamento.DataFim)),
                SqlHelper.Param("@Id", financiamento.FinanciamentoId));
        }

        public void Excluir(int financiamentoId)
        {
            SqlHelper.Execute("DELETE dbo.Financiamento WHERE FinanciamentoId = @Id",
                SqlHelper.Param("@Id", financiamentoId));
        }

        public decimal TotalDoProjeto(int projetoId)
        {
            return SqlHelper.ScalarDecimal(
                "SELECT ISNULL(SUM(Valor), 0) FROM dbo.Financiamento WHERE ProjetoId = @Id",
                SqlHelper.Param("@Id", projetoId));
        }

        public List<FinanciamentoResumo> ListarResumoPorProjeto(int projetoId)
        {
            return SqlHelper.Query(@"
SELECT f.FinanciamentoId, f.ProjetoId, pr.Titulo AS ProjetoTitulo,
       f.FonteId, fo.Nome AS FonteNome, fo.Sigla AS FonteSigla,
       f.Descricao, f.NumeroProcesso, f.Valor, f.DataInicio, f.DataFim
FROM dbo.Financiamento f
JOIN dbo.Projeto pr ON pr.ProjetoId = f.ProjetoId
JOIN dbo.FonteFinanciadora fo ON fo.FonteId = f.FonteId
WHERE f.ProjetoId = @Id
ORDER BY f.DataInicio DESC", Mappers.FinanciamentoResumo, SqlHelper.Param("@Id", projetoId));
        }

        public List<RelatorioContagem> ListarTotalPorProjeto()
        {
            return SqlHelper.Query(@"
SELECT pr.Titulo AS Rotulo, ISNULL(SUM(f.Valor), 0) AS Valor,
       COUNT(f.FinanciamentoId) AS Quantidade
FROM dbo.Projeto pr
LEFT JOIN dbo.Financiamento f ON f.ProjetoId = pr.ProjetoId
GROUP BY pr.ProjetoId, pr.Titulo
ORDER BY Valor DESC", r => new RelatorioContagem
            {
                Rotulo = DbValue.String(r, "Rotulo"),
                Quantidade = DbValue.Int(r, "Quantidade"),
                Valor = DbValue.Decimal(r, "Valor")
            });
        }

        public List<RelatorioContagem> ListarTotalPorFonte()
        {
            return SqlHelper.Query(@"
SELECT Nome AS Rotulo, QtdProjetos AS Quantidade, TotalCaptado AS Valor
FROM dbo.vw_FinanciamentoPorFonte
ORDER BY TotalCaptado DESC", r => new RelatorioContagem
            {
                Rotulo = DbValue.String(r, "Rotulo"),
                Quantidade = DbValue.Int(r, "Quantidade"),
                Valor = DbValue.Decimal(r, "Valor")
            });
        }

        public List<RelatorioFinanceiroLinha> ListarTotalPorPesquisador()
        {
            return SqlHelper.Query(@"
SELECT NomeCompleto AS Grupo, ProjetoTitulo AS Projeto, Papel, TotalCaptado AS Valor
FROM dbo.vw_FinanciamentoPorPesquisador
ORDER BY NomeCompleto, ProjetoTitulo", r => new RelatorioFinanceiroLinha
            {
                Grupo = DbValue.String(r, "Grupo"),
                Projeto = DbValue.String(r, "Projeto"),
                Papel = DbValue.String(r, "Papel"),
                Valor = DbValue.Decimal(r, "Valor")
            });
        }

        /* -------------------------- Fontes financiadoras ------------------------- */

        public List<FonteFinanciadora> ListarFontes()
        {
            return SqlHelper.Query("SELECT * FROM dbo.vw_FinanciamentoPorFonte ORDER BY Nome",
                Mappers.Fonte);
        }

        public FonteFinanciadora ObterFonte(int fonteId)
        {
            return SqlHelper.QuerySingle("SELECT * FROM dbo.vw_FinanciamentoPorFonte WHERE FonteId = @Id",
                Mappers.Fonte, SqlHelper.Param("@Id", fonteId));
        }

        public List<OpcaoSelecao> ListarOpcoesFontes()
        {
            return SqlHelper.Query(@"
SELECT FonteId AS Id, CASE WHEN Sigla IS NULL THEN Nome ELSE Sigla + N' — ' + Nome END AS Texto
FROM dbo.FonteFinanciadora ORDER BY Nome", r => new OpcaoSelecao
            {
                Id = DbValue.Int(r, "Id"),
                Texto = DbValue.String(r, "Texto")
            });
        }

        public int InserirFonte(FonteFinanciadora fonte)
        {
            return Convert.ToInt32(SqlHelper.Scalar(@"
INSERT dbo.FonteFinanciadora (Nome, Sigla, Tipo, SiteUrl)
VALUES (@Nome, @Sigla, @Tipo, @Site);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                SqlHelper.Param("@Nome", fonte.Nome.Trim()),
                SqlHelper.Param("@Sigla", DbValue.OrDBNull(fonte.Sigla)),
                SqlHelper.Param("@Tipo", fonte.Tipo),
                SqlHelper.Param("@Site", DbValue.OrDBNull(fonte.SiteUrl))));
        }

        public void AtualizarFonte(FonteFinanciadora fonte)
        {
            SqlHelper.Execute(@"
UPDATE dbo.FonteFinanciadora
SET Nome = @Nome, Sigla = @Sigla, Tipo = @Tipo, SiteUrl = @Site
WHERE FonteId = @Id",
                SqlHelper.Param("@Nome", fonte.Nome.Trim()),
                SqlHelper.Param("@Sigla", DbValue.OrDBNull(fonte.Sigla)),
                SqlHelper.Param("@Tipo", fonte.Tipo),
                SqlHelper.Param("@Site", DbValue.OrDBNull(fonte.SiteUrl)),
                SqlHelper.Param("@Id", fonte.FonteId));
        }

        public void ExcluirFonte(int fonteId)
        {
            SqlHelper.Execute("DELETE dbo.FonteFinanciadora WHERE FonteId = @Id",
                SqlHelper.Param("@Id", fonteId));
        }
    }
}
