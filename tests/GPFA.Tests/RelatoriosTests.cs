using System;
using System.Linq;
using GPFA.Business;
using GPFA.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class RelatoriosTests
    {
        [TestMethod]
        public void Indicadores_DevemSerCoerentesComAsViews()
        {
            var indicadores = AppServices.Relatorios.ObterIndicadores();

            Assert.AreEqual(Convert.ToInt32(TestDatabase.Escalar("SELECT COUNT(*) FROM dbo.Pesquisador")),
                indicadores.TotalPesquisadores);
            Assert.AreEqual(Convert.ToInt32(TestDatabase.Escalar("SELECT COUNT(*) FROM dbo.Estudante")),
                indicadores.TotalEstudantes);
            Assert.AreEqual(Convert.ToInt32(TestDatabase.Escalar("SELECT COUNT(*) FROM dbo.Projeto")),
                indicadores.TotalProjetos);
            Assert.AreEqual(Convert.ToInt32(TestDatabase.Escalar("SELECT COUNT(*) FROM dbo.Publicacao")),
                indicadores.TotalPublicacoes);
        }

        [TestMethod]
        public void ProjetosPorSituacao_DevemSomarTotalDeProjetos()
        {
            var lista = AppServices.Relatorios.ProjetosPorSituacao();
            var total = Convert.ToInt32(TestDatabase.Escalar("SELECT COUNT(*) FROM dbo.Projeto"));

            Assert.AreEqual(total, lista.Sum(x => x.Quantidade));
            Assert.IsTrue(lista.Sum(x => x.Percentual) > 99.9m && lista.Sum(x => x.Percentual) < 100.1m);
        }

        [TestMethod]
        public void FinanciamentosPorFonte_DevemSomarTotalGeral()
        {
            var lista = AppServices.Relatorios.FinanciamentosPorFonte();
            var totalView = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT ISNULL(SUM(TotalCaptado), 0) FROM dbo.vw_FinanciamentoPorFonte"));
            var totalRegistros = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT ISNULL(SUM(Valor), 0) FROM dbo.Financiamento"));

            Assert.AreEqual(totalRegistros, totalView);
            Assert.AreEqual(totalRegistros, lista.Sum(x => x.Valor ?? 0));
        }

        [TestMethod]
        public void PublicacoesPorAno_DevemSomarTotalDePublicacoes()
        {
            var lista = AppServices.Relatorios.PublicacoesPorAno();
            var total = Convert.ToInt32(TestDatabase.Escalar("SELECT COUNT(*) FROM dbo.Publicacao"));

            Assert.AreEqual(total, lista.Sum(x => x.Quantidade));
            Assert.IsTrue(lista.Any(x => x.Rotulo == "2026"));
        }

        [TestMethod]
        public void OrientandosPorOrientador_DevemRefletirAtuaisEPassados()
        {
            var lista = AppServices.Relatorios.OrientandosPorOrientador();
            var helena = lista.SingleOrDefault(x => x.OrientadorId == 1);

            Assert.IsNotNull(helena);
            Assert.IsTrue(helena.Atuais >= 0 && helena.Passados >= 1);
            Assert.AreEqual(helena.Atuais + helena.Passados, helena.Total);
        }

        [TestMethod]
        public void FinanciamentoPorPesquisador_DeveDetalharProjetosDistintos()
        {
            var linhas = AppServices.Relatorios.FinanciamentosPorPesquisador();
            var helena = linhas.Where(x => x.Grupo == "Helena Marques Ferreira").ToList();

            Assert.IsTrue(helena.Count >= 4);
            Assert.AreEqual(helena.Select(x => x.Projeto).Distinct().Count(), helena.Count,
                "Cada projeto deve aparecer uma única vez por pesquisador.");
            Assert.IsTrue(helena.Sum(x => x.Valor) >= 570000m);
        }

        [TestMethod]
        public void AlertasDeIntegridade_NaoDevemExistirAposOperacoesValidas()
        {
            var alertas = AppServices.Relatorios.AlertasIntegridade();
            Assert.AreEqual(0, alertas.Count,
                "Foram encontrados alertas: " + string.Join(" | ", alertas.Select(a => a.Alerta + ": " + a.Detalhe)));
        }

        [TestMethod]
        public void LinhasDePesquisa_DevemEstarDisponiveisParaFiltros()
        {
            var linhas = AppServices.Relatorios.LinhasPesquisa();
            Assert.IsTrue(linhas.Count >= 4);
            Assert.IsTrue(linhas.Contains("Materiais Quânticos"));
        }
    }
}
