using System;
using GPFA.Business;
using GPFA.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class FinanciamentoTests
    {
        [TestMethod]
        public void Financiamento_ValorZeroOuNegativoDeveSerBloqueado()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            var fonteId = TestData.CriarFonte();
            try
            {
                Assert.ThrowsException<ValidacaoException>(() =>
                    TestData.CriarFinanciamento(projetoId, fonteId, 0m));
                Assert.ThrowsException<ValidacaoException>(() =>
                    TestData.CriarFinanciamento(projetoId, fonteId, -100m));
            }
            finally
            {
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Financiamento_DeveSomarPorProjetoSemDuplicidade()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            var fonte1 = TestData.CriarFonte();
            var fonte2 = TestData.CriarFonte();
            try
            {
                var f1 = TestData.CriarFinanciamento(projetoId, fonte1, 10000.50m, "PROC-" + TestData.Sufixo());
                var f2 = TestData.CriarFinanciamento(projetoId, fonte2, 25000.25m, "PROC-" + TestData.Sufixo());

                Assert.AreEqual(35000.75m, AppServices.Financiamentos.TotalDoProjeto(projetoId));

                var resumo = AppServices.Projetos.ObterResumo(projetoId);
                Assert.AreEqual(35000.75m, resumo.TotalCaptado);

                AppServices.Financiamentos.Excluir(f1, TestData.Admin());
                Assert.AreEqual(25000.25m, AppServices.Financiamentos.TotalDoProjeto(projetoId));

                AppServices.Financiamentos.Excluir(f2, TestData.Admin());
                Assert.AreEqual(0m, AppServices.Financiamentos.TotalDoProjeto(projetoId));
            }
            finally
            {
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Financiamento_ProcessoDuplicadoNaMesmaFonteDeveSerBloqueado()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            var fonteId = TestData.CriarFonte();
            var processo = "DUP-" + TestData.Sufixo();
            var id = TestData.CriarFinanciamento(projetoId, fonteId, 5000m, processo);
            try
            {
                Assert.ThrowsException<IntegridadeDadosException>(() =>
                    TestData.CriarFinanciamento(projetoId, fonteId, 7000m, processo));
            }
            finally
            {
                AppServices.Financiamentos.Excluir(id, TestData.Admin());
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Fonte_ComFinanciamentoNaoPodeSerExcluida()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            var fonteId = TestData.CriarFonte();
            var financiamentoId = TestData.CriarFinanciamento(projetoId, fonteId, 9000m);
            try
            {
                var erro = Assert.ThrowsException<IntegridadeDadosException>(() =>
                    AppServices.Financiamentos.ExcluirFonte(fonteId, TestData.Admin()));
                StringAssert.Contains(erro.Message, "financiamento");
            }
            finally
            {
                AppServices.Financiamentos.Excluir(financiamentoId, TestData.Admin());
                AppServices.Financiamentos.ExcluirFonte(fonteId, TestData.Admin());
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Financiamento_CoordenadorSoGerenciaProjetosProprios()
        {
            var coordA = TestData.CriarPesquisador();
            var coordB = TestData.CriarPesquisador();
            var projetoA = TestData.CriarProjeto(coordA);
            var projetoB = TestData.CriarProjeto(coordB);
            var fonte = TestData.CriarFonte();
            var sessaoA = new UsuarioSessao
            {
                UsuarioId = 997,
                Login = "coord.fin",
                NomeCompleto = "Coordenador Financiamento",
                PerfilId = Dominio.Perfil.Coordenador,
                PerfilNome = Dominio.Perfil.CoordenadorNome,
                PesquisadorId = coordA
            };

            var idA = 0;
            try
            {
                idA = TestData.CriarFinanciamento(projetoA, fonte, 1234m, "A-" + TestData.Sufixo(), sessaoA);

                Assert.ThrowsException<RegraNegocioException>(() =>
                    TestData.CriarFinanciamento(projetoB, fonte, 4321m, "B-" + TestData.Sufixo(), sessaoA));
            }
            finally
            {
                if (idA > 0) AppServices.Financiamentos.Excluir(idA, TestData.Admin());
                AppServices.Financiamentos.ExcluirFonte(fonte, TestData.Admin());
                TentarExcluirProjeto(projetoA);
                TentarExcluirProjeto(projetoB);
                TentarExcluirPesquisador(coordA);
                TentarExcluirPesquisador(coordB);
            }
        }

        [TestMethod]
        public void Financiamento_DataFinalAnteriorAoInicioDeveSerBloqueada()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            var fonteId = TestData.CriarFonte();
            try
            {
                var financiamento = new Financiamento
                {
                    ProjetoId = projetoId,
                    FonteId = fonteId,
                    Descricao = "Datas inválidas",
                    Valor = 1000m,
                    DataInicio = DateTime.Today,
                    DataFim = DateTime.Today.AddDays(-10)
                };

                Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Financiamentos.Salvar(financiamento, TestData.Admin()));
            }
            finally
            {
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Financiamento_TotalGeralDoGrupoDeveSerIgualSomaDosRegistros()
        {
            var totalGeral = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT ISNULL(SUM(Valor), 0) FROM dbo.Financiamento"));
            var totalView = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT ISNULL(SUM(TotalCaptado), 0) FROM dbo.vw_FinanciamentoPorProjeto"));

            Assert.AreEqual(totalGeral, totalView);
        }

        private static void TentarExcluirProjeto(int id)
        {
            try { AppServices.Projetos.Excluir(id, TestData.Admin()); } catch { }
        }

        private static void TentarExcluirPesquisador(int id)
        {
            try { AppServices.Pesquisadores.Excluir(id); } catch { }
        }
    }
}
