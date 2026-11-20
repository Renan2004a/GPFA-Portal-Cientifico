using System;
using System.Collections.Generic;
using System.Linq;
using GPFA.Business;
using GPFA.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class EstudanteRegrasTests
    {
        [TestMethod]
        public void Estudante_SemProjetoNaoPodeSerCriado()
        {
            var orientadorId = TestData.CriarPesquisador();
            try
            {
                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Estudantes.Criar(TestData.NovoEstudante(), orientadorId, new List<int>(), "Bolsa"));

                StringAssert.Contains(erro.Message, "projeto");
            }
            finally
            {
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_SemOrientadorPrincipalNaoPodeSerCriado()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            try
            {
                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Estudantes.Criar(TestData.NovoEstudante(), 0, new List<int> { projetoId }, "Bolsa"));

                StringAssert.Contains(erro.Message, "orientador");
            }
            finally
            {
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_DeveSerCriadoComOrientacaoEProjetoAtivos()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            var estudanteId = TestData.CriarEstudante(orientadorId, projetoId);
            try
            {
                var perfil = AppServices.Estudantes.ObterPerfil(estudanteId);
                Assert.IsNotNull(perfil);
                Assert.AreEqual(orientadorId, perfil.Estudante.OrientadorId);
                Assert.AreEqual(1, perfil.Estudante.QtdProjetosAtivos);
                Assert.IsNotNull(perfil.OrientacaoAtual);
                Assert.AreEqual(orientadorId, perfil.OrientacaoAtual.OrientadorId);
                Assert.IsTrue(perfil.OrientacaoAtual.Principal);
                Assert.IsNull(perfil.OrientacaoAtual.DataFim);
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_NaoPodeFicarSemUltimoProjeto()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            var estudanteId = TestData.CriarEstudante(orientadorId, projetoId);
            try
            {
                var vinculo = AppServices.Projetos.ListarVinculosEstudantes(projetoId).Single();
                var erro = Assert.ThrowsException<IntegridadeDadosException>(() =>
                    AppServices.Projetos.EncerrarVinculoEstudante(vinculo.EstudanteProjetoId, DateTime.Today, TestData.Admin()));

                StringAssert.Contains(erro.Message, "projeto");
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_NaoPodeFicarSemOrientadorPrincipal()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            var estudanteId = TestData.CriarEstudante(orientadorId, projetoId);
            try
            {
                var orientacao = AppServices.Estudantes.ListarOrientacoes(estudanteId)
                    .Single(o => o.Principal && o.DataFim == null);

                var erro = Assert.ThrowsException<IntegridadeDadosException>(() =>
                    AppServices.Estudantes.EncerrarOrientacao(orientacao.OrientacaoId, DateTime.Today));

                StringAssert.Contains(erro.Message, "orientador");
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_PodeTrocarOrientadorPrincipalMantendoHistorico()
        {
            var orientador1 = TestData.CriarPesquisador();
            var orientador2 = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientador1);
            var estudanteId = TestData.CriarEstudante(orientador1, projetoId);
            try
            {
                AppServices.Estudantes.DefinirOrientadorPrincipal(estudanteId, orientador2,
                    DateTime.Today, "Troca de orientação para teste.");

                var perfil = AppServices.Estudantes.ObterPerfil(estudanteId);
                Assert.AreEqual(orientador2, perfil.OrientacaoAtual.OrientadorId);
                Assert.AreEqual(2, perfil.Orientacoes.Count);
                Assert.AreEqual(1, perfil.Orientacoes.Count(o => o.Principal && o.DataFim == null));
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientador1);
                AppServices.Pesquisadores.Excluir(orientador2);
            }
        }

        [TestMethod]
        public void Estudante_ConclusaoEncerraOrientacaoEVinculos()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            var estudanteId = TestData.CriarEstudante(orientadorId, projetoId);
            try
            {
                AppServices.Estudantes.Concluir(estudanteId, DateTime.Today);

                var perfil = AppServices.Estudantes.ObterPerfil(estudanteId);
                Assert.AreEqual(Dominio.SituacaoEstudante.Concluido, perfil.Estudante.Situacao);
                Assert.IsNull(perfil.OrientacaoAtual);
                Assert.IsTrue(perfil.Orientacoes.All(o => o.DataFim.HasValue));
                Assert.AreEqual(0, perfil.Estudante.QtdProjetosAtivos);
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_ReativacaoSemVinculosDeveSerBloqueada()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            var estudanteId = TestData.CriarEstudante(orientadorId, projetoId);
            try
            {
                AppServices.Estudantes.Concluir(estudanteId, DateTime.Today);
                var estudante = AppServices.Estudantes.ObterEntidade(estudanteId);
                estudante.Situacao = Dominio.SituacaoEstudante.Ativo;
                estudante.DataConclusao = null;

                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Estudantes.Atualizar(estudante));
                StringAssert.Contains(erro.Message, "orientador");
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_PodeTerMaisDeUmProjetoEEncerrarApenasUm()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projeto1 = TestData.CriarProjeto(orientadorId);
            var projeto2 = TestData.CriarProjeto(orientadorId);
            var estudanteId = TestData.CriarEstudante(orientadorId, projeto1);
            try
            {
                AppServices.Projetos.VincularEstudante(projeto2, estudanteId, DateTime.Today, "Segundo projeto", TestData.Admin());
                Assert.AreEqual(2, AppServices.Estudantes.ObterResumo(estudanteId).QtdProjetosAtivos);

                var vinculo1 = AppServices.Projetos.ListarVinculosEstudantes(projeto1)
                    .Single(v => v.EstudanteId == estudanteId);
                AppServices.Projetos.EncerrarVinculoEstudante(vinculo1.EstudanteProjetoId, DateTime.Today, TestData.Admin());

                var perfil = AppServices.Estudantes.ObterPerfil(estudanteId);
                Assert.AreEqual(1, perfil.Estudante.QtdProjetosAtivos);
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projeto1, TestData.Admin());
                AppServices.Projetos.Excluir(projeto2, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_NaoPodeSerOrientadoPorPesquisadorInativo()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            try
            {
                var orientador = AppServices.Pesquisadores.ObterEntidade(orientadorId);
                orientador.Ativo = false;
                AppServices.Pesquisadores.Salvar(orientador);

                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Estudantes.Criar(TestData.NovoEstudante(), orientadorId,
                        new List<int> { projetoId }, "Bolsa"));
                StringAssert.Contains(erro.Message, "orientador");
            }
            finally
            {
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }

        [TestMethod]
        public void Estudante_EmailDuplicadoDeveSerRejeitado()
        {
            var orientadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(orientadorId);
            var estudanteId = TestData.CriarEstudante(orientadorId, projetoId);
            try
            {
                var outro = TestData.NovoEstudante();
                outro.Email = AppServices.Estudantes.ObterEntidade(estudanteId).Email;

                Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Estudantes.Criar(outro, orientadorId, new List<int> { projetoId }, "Bolsa"));
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                AppServices.Pesquisadores.Excluir(orientadorId);
            }
        }
    }
}
