using System;
using GPFA.Business;
using GPFA.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class PublicacaoDestaqueTests
    {
        [TestMethod]
        public void Publicacao_ExigeAoMenosUmAutor()
        {
            var autorId = TestData.CriarPesquisador();
            try
            {
                var publicacao = TestData.NovaPublicacao(autorId);
                publicacao.Autores.Clear();

                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Publicacoes.Salvar(publicacao, TestData.Admin()));
                StringAssert.Contains(erro.Message, "autor");
            }
            finally
            {
                TentarExcluirPesquisador(autorId);
            }
        }

        [TestMethod]
        public void Publicacao_ExigeDoiOuLink()
        {
            var autorId = TestData.CriarPesquisador();
            try
            {
                var publicacao = TestData.NovaPublicacao(autorId);
                publicacao.Doi = null;
                publicacao.LinkAcesso = null;

                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Publicacoes.Salvar(publicacao, TestData.Admin()));
                StringAssert.Contains(erro.Message, "DOI");
            }
            finally
            {
                TentarExcluirPesquisador(autorId);
            }
        }

        [TestMethod]
        public void Publicacao_DevePermitirCriarAtualizarEExcluir()
        {
            var autor1 = TestData.CriarPesquisador();
            var autor2 = TestData.CriarPesquisador();
            var publicacaoId = 0;
            try
            {
                var publicacao = TestData.NovaPublicacao(autor1);
                publicacao.Autores.Add(new PublicacaoAutor { PesquisadorId = autor2 });
                publicacaoId = AppServices.Publicacoes.Salvar(publicacao, TestData.Admin());

                var salva = AppServices.Publicacoes.ObterEntidade(publicacaoId);
                Assert.AreEqual(2, salva.Autores.Count);

                salva.Titulo = "Título atualizado pelo teste";
                salva.Autores.RemoveAt(1);
                AppServices.Publicacoes.Salvar(salva, TestData.Admin());

                var atualizada = AppServices.Publicacoes.ObterEntidade(publicacaoId);
                Assert.AreEqual("Título atualizado pelo teste", atualizada.Titulo);
                Assert.AreEqual(1, atualizada.Autores.Count);

                AppServices.Publicacoes.Excluir(publicacaoId, TestData.Admin());
                Assert.IsNull(AppServices.Publicacoes.ObterEntidade(publicacaoId));
                publicacaoId = 0;
            }
            finally
            {
                if (publicacaoId > 0) TentarExcluirPublicacao(publicacaoId);
                TentarExcluirPesquisador(autor1);
                TentarExcluirPesquisador(autor2);
            }
        }

        [TestMethod]
        public void Destaque_SoPodeSerDePublicacaoDoProprioAutor()
        {
            var autor = TestData.CriarPesquisador();
            var outro = TestData.CriarPesquisador();
            var publicacaoId = TestData.CriarPublicacao(autor);
            try
            {
                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Publicacoes.AdicionarDestaque(outro, publicacaoId, 1, TestData.Admin()));
                StringAssert.Contains(erro.Message, "autor");
            }
            finally
            {
                TentarExcluirPublicacao(publicacaoId);
                TentarExcluirPesquisador(autor);
                TentarExcluirPesquisador(outro);
            }
        }

        [TestMethod]
        public void Destaque_NaoPodeRepetirPosicao()
        {
            var autor = TestData.CriarPesquisador();
            var pub1 = TestData.CriarPublicacao(autor);
            var pub2 = TestData.CriarPublicacao(autor);
            try
            {
                AppServices.Publicacoes.AdicionarDestaque(autor, pub1, 1, TestData.Admin());
                var erro = Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Publicacoes.AdicionarDestaque(autor, pub2, 1, TestData.Admin()));
                StringAssert.Contains(erro.Message, "posição");
            }
            finally
            {
                TentarExcluirPublicacao(pub1);
                TentarExcluirPublicacao(pub2);
                TentarExcluirPesquisador(autor);
            }
        }

        [TestMethod]
        public void ExcluirPublicacao_DeveRemoverDestaquesAssociados()
        {
            var autor = TestData.CriarPesquisador();
            var publicacaoId = TestData.CriarPublicacao(autor);
            try
            {
                AppServices.Publicacoes.AdicionarDestaque(autor, publicacaoId, 1, TestData.Admin());
                AppServices.Publicacoes.Excluir(publicacaoId, TestData.Admin());

                var restantes = Convert.ToInt32(TestDatabase.Escalar(
                    "SELECT COUNT(*) FROM dbo.DestaquePublicacao WHERE PublicacaoId = @Id",
                    new System.Data.SqlClient.SqlParameter("@Id", publicacaoId)));
                Assert.AreEqual(0, restantes);
            }
            finally
            {
                TentarExcluirPesquisador(autor);
            }
        }

        [TestMethod]
        public void Publicacao_CoordenadorSoGerenciaProjetosProprios()
        {
            var coordA = TestData.CriarPesquisador();
            var coordB = TestData.CriarPesquisador();
            var projetoA = TestData.CriarProjeto(coordA);
            var projetoB = TestData.CriarProjeto(coordB);
            var pubA = TestData.CriarPublicacao(coordA, projetoA);
            var pubB = TestData.CriarPublicacao(coordB, projetoB);

            var sessaoA = new UsuarioSessao
            {
                UsuarioId = 998,
                Login = "coord.pub",
                NomeCompleto = "Coordenador Publicações",
                PerfilId = Dominio.Perfil.Coordenador,
                PerfilNome = Dominio.Perfil.CoordenadorNome,
                PesquisadorId = coordA
            };

            try
            {
                var publicacaoA = AppServices.Publicacoes.ObterEntidade(pubA);
                publicacaoA.Titulo = "Publicação atualizada pelo coordenador";
                AppServices.Publicacoes.Salvar(publicacaoA, sessaoA);

                var publicacaoB = AppServices.Publicacoes.ObterEntidade(pubB);
                publicacaoB.Titulo = "Tentativa indevida";
                Assert.ThrowsException<RegraNegocioException>(() =>
                    AppServices.Publicacoes.Salvar(publicacaoB, sessaoA));

                Assert.ThrowsException<RegraNegocioException>(() =>
                    AppServices.Publicacoes.Excluir(pubB, sessaoA));
            }
            finally
            {
                TentarExcluirPublicacao(pubA);
                TentarExcluirPublicacao(pubB);
                TentarExcluirProjeto(projetoA);
                TentarExcluirProjeto(projetoB);
                TentarExcluirPesquisador(coordA);
                TentarExcluirPesquisador(coordB);
            }
        }

        [TestMethod]
        public void Publicacao_PodeExistirSemProjetoVinculado()
        {
            var autor = TestData.CriarPesquisador();
            var publicacaoId = 0;
            try
            {
                publicacaoId = TestData.CriarPublicacao(autor, null);
                var publicacao = AppServices.Publicacoes.ObterEntidade(publicacaoId);
                Assert.IsNull(publicacao.ProjetoId);
            }
            finally
            {
                if (publicacaoId > 0) TentarExcluirPublicacao(publicacaoId);
                TentarExcluirPesquisador(autor);
            }
        }

        private static void TentarExcluirPublicacao(int id)
        {
            try { AppServices.Publicacoes.Excluir(id, TestData.Admin()); } catch { }
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
