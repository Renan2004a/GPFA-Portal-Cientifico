using System;
using System.Linq;
using GPFA.Business;
using GPFA.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class ProjetoRegrasTests
    {
        [TestMethod]
        public void Projeto_DevePermitirCrudCompleto()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = 0;
            try
            {
                projetoId = TestData.CriarProjeto(coordenadorId);
                var resumo = AppServices.Projetos.ObterResumo(projetoId);
                Assert.IsNotNull(resumo);
                Assert.AreEqual(coordenadorId, resumo.CoordenadorId);

                var projeto = AppServices.Projetos.ObterEntidade(projetoId);
                projeto.Resumo = "Resumo atualizado pelo teste de integração.";
                projeto.Situacao = Dominio.SituacaoProjeto.Suspenso;
                AppServices.Projetos.Salvar(projeto, TestData.Admin());

                var atualizado = AppServices.Projetos.ObterResumo(projetoId);
                Assert.AreEqual("Resumo atualizado pelo teste de integração.", atualizado.Resumo);
                Assert.AreEqual(Dominio.SituacaoProjeto.Suspenso, atualizado.Situacao);

                AppServices.Projetos.Excluir(projetoId, TestData.Admin());
                Assert.IsNull(AppServices.Projetos.ObterResumo(projetoId));
                projetoId = 0;
            }
            finally
            {
                if (projetoId > 0) TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Projeto_ConcluidoExigeDataDeFim()
        {
            var coordenadorId = TestData.CriarPesquisador();
            try
            {
                var projeto = TestData.NovoProjeto(coordenadorId, Dominio.SituacaoProjeto.Concluido);
                projeto.DataFim = null;

                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Projetos.Salvar(projeto, TestData.Admin()));
                StringAssert.Contains(erro.Message, "data de término");
            }
            finally
            {
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Projeto_DeveExigirCoordenadorCadastrado()
        {
            var projeto = TestData.NovoProjeto(999999);
            Assert.ThrowsException<ValidacaoException>(() =>
                AppServices.Projetos.Salvar(projeto, TestData.Admin()));
        }

        [TestMethod]
        public void Projeto_CoordenadorNaoPodeSerParticipanteDuplicado()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            try
            {
                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Projetos.AdicionarParticipante(projetoId, coordenadorId, "Pesquisador",
                        DateTime.Today, TestData.Admin()));
                StringAssert.Contains(erro.Message, "coordenador");
            }
            finally
            {
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Projeto_ParticipacaoDuplicadaDeveSerBloqueada()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var participanteId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            try
            {
                AppServices.Projetos.AdicionarParticipante(projetoId, participanteId, "Colaborador",
                    DateTime.Today, TestData.Admin());

                Assert.ThrowsException<IntegridadeDadosException>(() =>
                    AppServices.Projetos.AdicionarParticipante(projetoId, participanteId, "Colaborador",
                        DateTime.Today, TestData.Admin()));
            }
            finally
            {
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(participanteId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Projeto_CoordenadorSoGerenciaPropriosProjetos()
        {
            var coordA = TestData.CriarPesquisador();
            var coordB = TestData.CriarPesquisador();
            var projetoA = TestData.CriarProjeto(coordA);
            var projetoB = TestData.CriarProjeto(coordB);
            var sessaoA = new UsuarioSessao
            {
                UsuarioId = 999,
                Login = "coord.teste",
                NomeCompleto = "Coordenador de Teste",
                PerfilId = Dominio.Perfil.Coordenador,
                PerfilNome = Dominio.Perfil.CoordenadorNome,
                PesquisadorId = coordA
            };
            try
            {
                Assert.IsTrue(AppServices.Projetos.PodeGerenciar(sessaoA, projetoA));
                Assert.IsFalse(AppServices.Projetos.PodeGerenciar(sessaoA, projetoB));

                var projeto = AppServices.Projetos.ObterEntidade(projetoB);
                projeto.Resumo = "Tentativa indevida de edição por outro coordenador.";
                Assert.ThrowsException<RegraNegocioException>(() =>
                    AppServices.Projetos.Salvar(projeto, sessaoA));
            }
            finally
            {
                TentarExcluirProjeto(projetoA);
                TentarExcluirProjeto(projetoB);
                TentarExcluirPesquisador(coordA);
                TentarExcluirPesquisador(coordB);
            }
        }

        [TestMethod]
        public void Projeto_ComFinanciamentoNaoPodeSerExcluido()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            var fonteId = TestData.CriarFonte();
            var financiamentoId = TestData.CriarFinanciamento(projetoId, fonteId, 1500m);
            try
            {
                var erro = Assert.ThrowsException<IntegridadeDadosException>(() =>
                    AppServices.Projetos.Excluir(projetoId, TestData.Admin()));
                StringAssert.Contains(erro.Message, "financiamento");
            }
            finally
            {
                AppServices.Financiamentos.Excluir(financiamentoId, TestData.Admin());
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Projeto_ComEstudanteNaoPodeSerExcluido()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            var estudanteId = TestData.CriarEstudante(coordenadorId, projetoId);
            try
            {
                var erro = Assert.ThrowsException<IntegridadeDadosException>(() =>
                    AppServices.Projetos.Excluir(projetoId, TestData.Admin()));
                StringAssert.Contains(erro.Message, "estudante");
            }
            finally
            {
                AppServices.Estudantes.Excluir(estudanteId);
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Projeto_ComPublicacaoNaoPodeSerExcluido()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            var publicacaoId = TestData.CriarPublicacao(coordenadorId, projetoId);
            try
            {
                var erro = Assert.ThrowsException<IntegridadeDadosException>(() =>
                    AppServices.Projetos.Excluir(projetoId, TestData.Admin()));
                StringAssert.Contains(erro.Message, "publica");
            }
            finally
            {
                AppServices.Publicacoes.Excluir(publicacaoId, TestData.Admin());
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
            }
        }

        [TestMethod]
        public void Projeto_AdministradorPodeAdicionarMultiplosParticipantes()
        {
            var coordenadorId = TestData.CriarPesquisador();
            var p1 = TestData.CriarPesquisador();
            var p2 = TestData.CriarPesquisador();
            var projetoId = TestData.CriarProjeto(coordenadorId);
            try
            {
                AppServices.Projetos.AdicionarParticipante(projetoId, p1, "Colaborador", DateTime.Today, TestData.Admin());
                AppServices.Projetos.AdicionarParticipante(projetoId, p2, "Colaborador", DateTime.Today, TestData.Admin());

                var resumo = AppServices.Projetos.ObterResumo(projetoId);
                Assert.AreEqual(3, resumo.QtdPesquisadores, "Coordenador + dois participantes.");
            }
            finally
            {
                TentarExcluirProjeto(projetoId);
                TentarExcluirPesquisador(coordenadorId);
                TentarExcluirPesquisador(p1);
                TentarExcluirPesquisador(p2);
            }
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
