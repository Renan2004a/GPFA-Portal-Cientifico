using System.Collections.Generic;
using GPFA.Models;

namespace GPFA.Data
{
    public class RelatorioRepository
    {
        public DashboardIndicadores ObterIndicadores()
        {
            return SqlHelper.QuerySingle("SELECT * FROM dbo.vw_IndicadoresGerais", r => new DashboardIndicadores
            {
                TotalPesquisadores = DbValue.Int(r, "TotalPesquisadores"),
                TotalEstudantes = DbValue.Int(r, "TotalEstudantes"),
                TotalEstudantesAtivos = DbValue.Int(r, "TotalEstudantesAtivos"),
                TotalProjetos = DbValue.Int(r, "TotalProjetos"),
                TotalProjetosEmAndamento = DbValue.Int(r, "TotalProjetosEmAndamento"),
                TotalPublicacoes = DbValue.Int(r, "TotalPublicacoes"),
                TotalFinanciamentos = DbValue.Int(r, "TotalFinanciamentos"),
                TotalCaptado = DbValue.Decimal(r, "TotalCaptado"),
                TotalFontes = DbValue.Int(r, "TotalFontes")
            });
        }

        public List<RelatorioContagem> ListarProjetosPorSituacao()
        {
            return SqlHelper.Query(@"
SELECT Situacao AS Rotulo, Quantidade, NULL AS Valor
FROM dbo.vw_ProjetosPorSituacao
ORDER BY Quantidade DESC", r => new RelatorioContagem
            {
                Rotulo = DbValue.String(r, "Rotulo"),
                Quantidade = DbValue.Int(r, "Quantidade"),
                Valor = null
            });
        }

        public List<RelatorioContagem> ListarPublicacoesPorAno()
        {
            return SqlHelper.Query(@"
SELECT CAST(Ano AS NVARCHAR(10)) AS Rotulo, Quantidade, NULL AS Valor
FROM dbo.vw_PublicacoesPorAno ORDER BY Ano DESC", r => new RelatorioContagem
            {
                Rotulo = DbValue.String(r, "Rotulo"),
                Quantidade = DbValue.Int(r, "Quantidade"),
                Valor = null
            });
        }

        public List<RelatorioContagem> ListarPublicacoesPorPesquisador()
        {
            return SqlHelper.Query(@"
SELECT NomeCompleto AS Rotulo, Quantidade, NULL AS Valor
FROM dbo.vw_PublicacoesPorPesquisador
ORDER BY Quantidade DESC, NomeCompleto", r => new RelatorioContagem
            {
                Rotulo = DbValue.String(r, "Rotulo"),
                Quantidade = DbValue.Int(r, "Quantidade"),
                Valor = null
            });
        }

        public List<OrientadorContagem> ListarOrientandosPorOrientador()
        {
            return SqlHelper.Query(@"
SELECT OrientadorId, OrientadorNome, Atuais, Passados
FROM dbo.vw_OrientandosPorOrientador
ORDER BY Atuais DESC, OrientadorNome", r => new OrientadorContagem
            {
                OrientadorId = DbValue.Int(r, "OrientadorId"),
                OrientadorNome = DbValue.String(r, "OrientadorNome"),
                Atuais = DbValue.Int(r, "Atuais"),
                Passados = DbValue.Int(r, "Passados")
            });
        }

        public List<AlertaIntegridade> ListarAlertasIntegridade()
        {
            return SqlHelper.Query(@"
SELECT Alerta, RegistroId, Detalhe FROM dbo.vw_AlertasIntegridade
ORDER BY Alerta, Detalhe", r => new AlertaIntegridade
            {
                Alerta = DbValue.String(r, "Alerta"),
                RegistroId = DbValue.Int(r, "RegistroId"),
                Detalhe = DbValue.String(r, "Detalhe")
            });
        }

        public List<string> ListarLinhasPesquisa()
        {
            return SqlHelper.Query(@"
SELECT DISTINCT LinhaPesquisa FROM dbo.Pesquisador
WHERE LinhaPesquisa IS NOT NULL AND LinhaPesquisa <> N''
ORDER BY LinhaPesquisa", r => DbValue.String(r, "LinhaPesquisa"));
        }

        public List<string> ListarAreasAtuacao()
        {
            return SqlHelper.Query(@"
SELECT DISTINCT AreaAtuacao FROM dbo.Pessoa
WHERE AreaAtuacao IS NOT NULL AND AreaAtuacao <> N'' AND TipoPessoa = 'P'
ORDER BY AreaAtuacao", r => DbValue.String(r, "AreaAtuacao"));
        }
    }
}
