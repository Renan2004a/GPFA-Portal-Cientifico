using System;
using GPFA.Business;
using GPFA.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class AutenticacaoTests
    {
        [TestMethod]
        public void Login_AdministradorDemonstracaoDeveFuncionar()
        {
            var resultado = AppServices.Auth.Autenticar("admin", "Gpfa@2026");

            Assert.IsTrue(resultado.Sucesso, resultado.Mensagem);
            Assert.AreEqual(Dominio.Perfil.Administrador, resultado.Usuario.PerfilId);
            Assert.AreEqual("Administrador", resultado.Usuario.PerfilNome);
        }

        [TestMethod]
        public void Login_CoordenadorDeveEstarVinculadoAPesquisador()
        {
            var resultado = AppServices.Auth.Autenticar("coordenador", "Coord@2026");

            Assert.IsTrue(resultado.Sucesso, resultado.Mensagem);
            Assert.AreEqual(Dominio.Perfil.Coordenador, resultado.Usuario.PerfilId);
            Assert.AreEqual(1, resultado.Usuario.PesquisadorId);
        }

        [TestMethod]
        public void Login_SenhaIncorretaDeveFalharSemRevelarDetalhes()
        {
            var resultado = AppServices.Auth.Autenticar("admin", "SenhaErrada@1");

            Assert.IsFalse(resultado.Sucesso);
            Assert.AreEqual("Login ou senha inválidos.", resultado.Mensagem);
        }

        [TestMethod]
        public void Login_UsuarioInexistenteDeveFalhar()
        {
            var resultado = AppServices.Auth.Autenticar("nao.existe." + TestData.Sufixo(), "Qualquer@123");
            Assert.IsFalse(resultado.Sucesso);
        }

        [TestMethod]
        public void Login_CincoFalhasDevemBloquearContaTemporariamente()
        {
            var login = TestData.CriarUsuario(Dominio.Perfil.Pesquisador, 2, "Bloqueio@123");
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    var falha = AppServices.Auth.Autenticar(login, "Errada@123");
                    Assert.IsFalse(falha.Sucesso);
                }

                var bloqueado = AppServices.Auth.Autenticar(login, "Bloqueio@123");
                Assert.IsFalse(bloqueado.Sucesso);
                StringAssert.Contains(bloqueado.Mensagem.ToLowerInvariant(), "bloqueada");
            }
            finally
            {
                var usuario = AppServices.Usuarios.Listar().Find(u => u.Login == login);
                if (usuario != null) AppServices.Usuarios.Excluir(usuario.UsuarioId, TestData.Admin());
            }
        }

        [TestMethod]
        public void Login_UsuarioInativoNaoPodeAcessar()
        {
            var login = TestData.CriarUsuario(Dominio.Perfil.Pesquisador, 2, "Inativo@123");
            Usuario usuario = null;
            try
            {
                usuario = AppServices.Usuarios.Listar().Find(u => u.Login == login);
                usuario.Ativo = false;
                AppServices.Usuarios.Salvar(usuario, null, TestData.Admin());

                var resultado = AppServices.Auth.Autenticar(login, "Inativo@123");
                Assert.IsFalse(resultado.Sucesso);
                StringAssert.Contains(resultado.Mensagem.ToLowerInvariant(), "inativo");
            }
            finally
            {
                if (usuario != null)
                {
                    usuario.Ativo = true;
                    AppServices.Usuarios.Excluir(usuario.UsuarioId, TestData.Admin());
                }
            }
        }

        [TestMethod]
        public void Senha_DeveSerAlteradaEValidada()
        {
            const string senhaInicial = "Inicial@123";
            const string senhaNova = "NovaSenha@456";
            var login = TestData.CriarUsuario(Dominio.Perfil.Pesquisador, 2, senhaInicial);
            try
            {
                var usuario = AppServices.Usuarios.Listar().Find(u => u.Login == login);
                Assert.IsNotNull(usuario);

                Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Auth.AlterarSenha(usuario.UsuarioId, "errada", senhaNova, senhaNova));
                Assert.ThrowsException<ValidacaoException>(() =>
                    AppServices.Auth.AlterarSenha(usuario.UsuarioId, senhaInicial, senhaNova, "Diferente@789"));

                AppServices.Auth.AlterarSenha(usuario.UsuarioId, senhaInicial, senhaNova, senhaNova);

                Assert.IsFalse(AppServices.Auth.Autenticar(login, senhaInicial).Sucesso);
                Assert.IsTrue(AppServices.Auth.Autenticar(login, senhaNova).Sucesso);
            }
            finally
            {
                var usuario = AppServices.Usuarios.Listar().Find(u => u.Login == login);
                if (usuario != null) AppServices.Usuarios.Excluir(usuario.UsuarioId, TestData.Admin());
            }
        }

        [TestMethod]
        public void Senhas_NaoPodemEstarEmTextoPuroNoBanco()
        {
            var hashAdmin = Convert.ToString(TestDatabase.Escalar(
                "SELECT SenhaHash FROM dbo.Usuario WHERE Login = N'admin'"));
            var hashCoord = Convert.ToString(TestDatabase.Escalar(
                "SELECT SenhaHash FROM dbo.Usuario WHERE Login = N'coordenador'"));

            Assert.IsFalse(hashAdmin.Contains("Gpfa@2026"));
            Assert.IsFalse(hashCoord.Contains("Coord@2026"));
            StringAssert.StartsWith(hashAdmin, "120000.");
        }

        [TestMethod]
        public void Usuario_UltimoAdministradorNaoPodeSerDesativadoNemExcluido()
        {
            var admin = AppServices.Usuarios.Obter(1);
            Assert.IsNotNull(admin);

            admin.Ativo = false;
            var erro = Assert.ThrowsException<RegraNegocioException>(() =>
                AppServices.Usuarios.Salvar(admin, null, TestData.Admin()));
            StringAssert.Contains(erro.Message, "administrador");

            admin.Ativo = true;
            Assert.ThrowsException<RegraNegocioException>(() =>
                AppServices.Usuarios.Excluir(1, TestData.Admin()));
        }

        [TestMethod]
        public void Login_NaoDeveSerVulneravelAInjecaoSql()
        {
            var resultado = AppServices.Auth.Autenticar("' OR 1=1 --", "qualquer");
            Assert.IsFalse(resultado.Sucesso);

            var usuarios = Convert.ToInt32(TestDatabase.Escalar("SELECT COUNT(*) FROM dbo.Usuario"));
            Assert.IsTrue(usuarios >= 3, "A tabela de usuários deve permanecer íntegra.");
        }

        [TestMethod]
        public void Usuario_CoordenadorExigePesquisadorVinculado()
        {
            var erro = Assert.ThrowsException<ValidacaoException>(() =>
                AppServices.Usuarios.Salvar(new Usuario
                {
                    Login = "coord.semvinculo." + TestData.Sufixo(),
                    NomeCompleto = "Coordenador Sem Vínculo",
                    Email = "coord.semvinculo." + TestData.Sufixo() + "@teste.local",
                    PerfilId = Dominio.Perfil.Coordenador,
                    PesquisadorId = null,
                    Ativo = true
                }, "Senha@123", TestData.Admin()));

            StringAssert.Contains(erro.Message, "vinculados");
        }
    }
}
