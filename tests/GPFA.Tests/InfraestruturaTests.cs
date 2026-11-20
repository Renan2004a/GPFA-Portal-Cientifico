using System;
using System.Data.SqlClient;
using GPFA.Business;
using GPFA.Business.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class InfraestruturaTests
    {
        [TestMethod]
        public void BancoDeTeste_DeveConterDadosDeDemonstracao()
        {
            var indicadores = AppServices.Relatorios.ObterIndicadores();

            Assert.IsTrue(indicadores.TotalPesquisadores >= 6, "Esperava ao menos 6 pesquisadores.");
            Assert.IsTrue(indicadores.TotalEstudantes >= 13, "Esperava ao menos 13 estudantes.");
            Assert.IsTrue(indicadores.TotalProjetos >= 6, "Esperava ao menos 6 projetos.");
            Assert.IsTrue(indicadores.TotalPublicacoes >= 14, "Esperava ao menos 14 publicações.");
            Assert.IsTrue(Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT TotalCaptado FROM dbo.vw_IndicadoresGerais")) >= 755000m,
                "O total inicial de financiamentos deve ser de ao menos R$ 755.000,00.");
        }

        [TestMethod]
        public void ViewDeProjeto_NaoDuplicaFinanciamentos()
        {
            var totalProjeto1 = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT TotalCaptado FROM dbo.vw_ProjetoResumo WHERE ProjetoId = 1"));
            var somaDireta = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT SUM(Valor) FROM dbo.Financiamento WHERE ProjetoId = 1"));

            Assert.AreEqual(250000m, totalProjeto1);
            Assert.AreEqual(somaDireta, totalProjeto1);
        }

        [TestMethod]
        public void ViewDeFonte_SomaCadaRegistroUmaVez()
        {
            var cnpq = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT TotalCaptado FROM dbo.vw_FinanciamentoPorFonte WHERE Sigla = N'CNPq'"));
            var somaDireta = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT SUM(f.Valor) FROM dbo.Financiamento f JOIN dbo.FonteFinanciadora fo ON fo.FonteId = f.FonteId WHERE fo.Sigla = N'CNPq'"));

            Assert.AreEqual(270000m, cnpq);
            Assert.AreEqual(somaDireta, cnpq);
        }

        [TestMethod]
        public void ViewDePesquisador_TotalHelenaConsideraProjetosDistintos()
        {
            var total = Convert.ToDecimal(TestDatabase.Escalar(
                "SELECT TotalCaptado FROM dbo.vw_PesquisadorResumo WHERE PesquisadorId = 1"));
            Assert.AreEqual(570000m, total);
        }

        [TestMethod]
        public void SomaDosTotaisPorPesquisador_PodeSerMaiorQueTotalGeral()
        {
            var totalGeral = Convert.ToDecimal(TestDatabase.Escalar("SELECT SUM(Valor) FROM dbo.Financiamento"));
            var totalPesquisadores = Convert.ToDecimal(TestDatabase.Escalar("SELECT SUM(TotalCaptado) FROM dbo.vw_PesquisadorResumo"));

            Assert.IsTrue(totalPesquisadores > totalGeral,
                "Projetos compartilhados devem aparecer no total de cada pesquisador, sem alterar o total geral.");
        }

        [TestMethod]
        public void Senha_DeveSerArmazenadaComHashEVerificadaCorretamente()
        {
            const string senha = "Senha@Forte123";
            var hash = PasswordHasher.Hash(senha);

            Assert.IsFalse(hash.Contains(senha));
            Assert.IsTrue(PasswordHasher.Verificar(senha, hash));
            Assert.IsFalse(PasswordHasher.Verificar("Senha@Forte124", hash));
        }

        [TestMethod]
        public void Senha_DeveRejeitarHashMalformado()
        {
            Assert.IsFalse(PasswordHasher.Verificar("qualquer", "texto-invalido"));
            Assert.IsFalse(PasswordHasher.Verificar("qualquer", "120000.invalido.invalido"));
        }

        [TestMethod]
        public void SenhaFraca_DeveSerRejeitada()
        {
            Assert.ThrowsException<GPFA.Models.ValidacaoException>(() => PasswordHasher.ValidarForca("12345678"));
            Assert.ThrowsException<GPFA.Models.ValidacaoException>(() => PasswordHasher.ValidarForca("apenasminusculas"));
            PasswordHasher.ValidarForca("Senha@123");
        }

        [TestMethod]
        public void ConsultaParametrizada_NaoDevePermitirInjecao()
        {
            var textoMalicioso = "Robert'); DROP TABLE dbo.Pesquisador;--";
            using (var conexao = TestDatabase.Abrir())
            using (var comando = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Pessoa WHERE NomeCompleto = @Nome", conexao))
            {
                comando.Parameters.AddWithValue("@Nome", textoMalicioso);
                Assert.AreEqual(0, Convert.ToInt32(comando.ExecuteScalar()));
            }

            var tabelaExiste = Convert.ToInt32(TestDatabase.Escalar(
                "SELECT COUNT(*) FROM sys.tables WHERE name = N'Pesquisador'"));
            Assert.AreEqual(1, tabelaExiste, "A tabela não pode ser afetada por conteúdo digitado pelo usuário.");
        }
    }
}
