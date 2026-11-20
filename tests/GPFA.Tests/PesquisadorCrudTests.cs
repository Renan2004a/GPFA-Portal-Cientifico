using System;
using System.Linq;
using GPFA.Business;
using GPFA.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class PesquisadorCrudTests
    {
        [TestMethod]
        public void Pesquisador_DevePermitirCrudCompleto()
        {
            var id = 0;
            try
            {
                var novo = TestData.NovoPesquisador();
                id = AppServices.Pesquisadores.Salvar(novo);
                Assert.IsTrue(id > 0);

                var resumo = AppServices.Pesquisadores.ObterResumo(id);
                Assert.IsNotNull(resumo);
                Assert.AreEqual(novo.NomeCompleto, resumo.NomeCompleto);

                var entidade = AppServices.Pesquisadores.ObterEntidade(id);
                entidade.NomeCompleto = "Pesquisador Atualizado " + TestData.Sufixo();
                entidade.MiniBiografia = "Biografia atualizada pelo teste.";
                AppServices.Pesquisadores.Salvar(entidade);

                var atualizado = AppServices.Pesquisadores.ObterEntidade(id);
                Assert.AreEqual(entidade.NomeCompleto, atualizado.NomeCompleto);
                Assert.AreEqual("Biografia atualizada pelo teste.", atualizado.MiniBiografia);

                AppServices.Pesquisadores.Excluir(id);
                Assert.IsNull(AppServices.Pesquisadores.ObterResumo(id));
                id = 0;
            }
            finally
            {
                if (id > 0) TentarExcluir(id);
            }
        }

        [TestMethod]
        public void Pesquisador_NaoDeveAceitarEmailDuplicado()
        {
            var id = TestData.CriarPesquisador();
            try
            {
                var outro = TestData.NovoPesquisador();
                outro.Email = AppServices.Pesquisadores.ObterEntidade(id).Email;

                Assert.ThrowsException<ValidacaoException>(() => AppServices.Pesquisadores.Salvar(outro));
            }
            finally
            {
                TentarExcluir(id);
            }
        }

        [TestMethod]
        public void Pesquisador_ComProjetoCoordenadoNaoPodeSerExcluido()
        {
            var pesquisadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(pesquisadorId);
            try
            {
                var erro = Assert.ThrowsException<IntegridadeDadosException>(
                    () => AppServices.Pesquisadores.Excluir(pesquisadorId));
                StringAssert.Contains(erro.Message, "projeto");
            }
            finally
            {
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                TentarExcluir(pesquisadorId);
            }
        }

        [TestMethod]
        public void Pesquisador_ComOrientandoNaoPodeSerExcluido()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            var estudanteId = TestData.CriarEstudante(orientadorId, projetoId);
            try
            {
                var erro = Assert.ThrowsException<IntegridadeDadosException>(
                    () => AppServices.Pesquisadores.Excluir(orientadorId));
                StringAssert.Contains(erro.Message, "orienta");
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                TentarExcluir(orientadorId);
            }
        }

        [TestMethod]
        public void Pesquisador_DeveRejeitarUrlPerigosa()
        {
            var pesquisador = TestData.NovoPesquisador();
            pesquisador.GoogleScholarUrl = "javascript:alert(1)";

            Assert.ThrowsException<ValidacaoException>(() => AppServices.Pesquisadores.Salvar(pesquisador));
        }

        [TestMethod]
        public void Pesquisador_NomeComApostrofoDeveSerTratadoComoTexto()
        {
            const string nome = "O'Brien Teste " + "X";
            var pesquisador = TestData.NovoPesquisador(nome);
            var id = AppServices.Pesquisadores.Salvar(pesquisador);
            try
            {
                var salvo = AppServices.Pesquisadores.ObterEntidade(id);
                Assert.AreEqual(nome, salvo.NomeCompleto);

                var tabelas = Convert.ToInt32(TestDatabase.Escalar(
                    "SELECT COUNT(*) FROM sys.tables WHERE name = N'Pesquisador'"));
                Assert.AreEqual(1, tabelas);
            }
            finally
            {
                TentarExcluir(id);
            }
        }

        [TestMethod]
        public void PerfilHelena_DeveSepararOrientandosAtuaisEPassados()
        {
            var perfil = AppServices.Pesquisadores.ObterPerfil(1);

            Assert.IsNotNull(perfil);
            Assert.AreEqual(5, perfil.Destaques.Count, "Helena deve ter exatamente 5 destaques.");
            Assert.AreEqual(5, perfil.Publicacoes.Count);
            Assert.IsTrue(perfil.OrientandosAtuais.Count >= 2);
            Assert.IsTrue(perfil.OrientandosPassados.Count >= 1);

            var idsAtuais = perfil.OrientandosAtuais.Select(o => o.EstudanteId).ToList();
            var idsPassados = perfil.OrientandosPassados.Select(o => o.EstudanteId).ToList();
            Assert.IsFalse(idsAtuais.Intersect(idsPassados).Any(),
                "Um estudante não pode aparecer simultaneamente como atual e passado.");
            Assert.IsTrue(perfil.OrientandosAtuais.All(o => o.Vinculo == "Atual"));
            Assert.IsTrue(perfil.OrientandosPassados.All(o => o.Vinculo == "Passado"));
        }

        [TestMethod]
        public void Destaques_NaoPodemUltrapassarCinco()
        {
            var pesquisadorId = TestData.CriarPesquisador();
            try
            {
                for (int i = 1; i <= 5; i++)
                {
                    var publicacaoId = TestData.CriarPublicacao(pesquisadorId);
                    AppServices.Publicacoes.AdicionarDestaque(pesquisadorId, publicacaoId, i, TestData.Admin());
                }

                var sexta = TestData.CriarPublicacao(pesquisadorId);
                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Publicacoes.AdicionarDestaque(pesquisadorId, sexta, 1, TestData.Admin()));
                StringAssert.Contains(erro.Message, "5");
            }
            finally
            {
                TentarExcluir(pesquisadorId);
            }
        }

        private static void TentarExcluir(int id)
        {
            try { AppServices.Pesquisadores.Excluir(id); }
            catch { }
        }
    }
}
