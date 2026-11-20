using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GPFA.Tests
{
    [TestClass]
    public class IntegridadeBancoTests
    {
        [TestMethod]
        public void Banco_DeveBloquearFinanciamentoNegativo()
        {
            Assert.ThrowsException<SqlException>(() => Executar(@"
INSERT dbo.Financiamento (ProjetoId, FonteId, Descricao, Valor, DataInicio, Moeda)
VALUES (1, 1, N'Valor negativo', -1, '2026-01-01', 'BRL');"));
        }

        [TestMethod]
        public void Banco_DeveBloquearSegundoOrientadorPrincipalAtivo()
        {
            Assert.ThrowsException<SqlException>(() => Executar(@"
INSERT dbo.Orientacao (EstudanteId, OrientadorId, Principal, DataInicio, DataFim)
VALUES (8, 1, 1, '2026-01-01', NULL);"));
        }

        [TestMethod]
        public void Banco_DeveBloquearCoordenadorComoParticipante()
        {
            Assert.ThrowsException<SqlException>(() => Executar(@"
INSERT dbo.ParticipacaoProjeto (ProjetoId, PesquisadorId, Papel, DataEntrada)
VALUES (1, 1, N'Coordenador duplicado', '2026-01-01');"));
        }

        [TestMethod]
        public void Banco_DeveBloquearRemocaoDoUltimoProjetoDeEstudanteAtivo()
        {
            Assert.ThrowsException<SqlException>(() => Executar(
                "DELETE dbo.EstudanteProjeto WHERE EstudanteId = 11;"));
        }

        [TestMethod]
        public void Banco_DeveBloquearRemocaoDaUltimaOrientacaoPrincipal()
        {
            Assert.ThrowsException<SqlException>(() => Executar(
                "DELETE dbo.Orientacao WHERE EstudanteId = 8 AND DataFim IS NULL;"));
        }

        [TestMethod]
        public void Banco_DeveBloquearExclusaoDePesquisadorComProjetos()
        {
            Assert.ThrowsException<SqlException>(() => Executar(
                "DELETE dbo.Pesquisador WHERE PesquisadorId = 1;"));
        }

        [TestMethod]
        public void Banco_DeveBloquearExclusaoDeProjetoComPublicacoes()
        {
            Assert.ThrowsException<SqlException>(() => Executar(
                "DELETE dbo.Projeto WHERE ProjetoId = 1;"));
        }

        [TestMethod]
        public void Banco_DeveValidarSituacaoDeProjeto()
        {
            Assert.ThrowsException<SqlException>(() => Executar(
                "UPDATE dbo.Projeto SET Situacao = N'Situação inexistente' WHERE ProjetoId = 2;"));
        }

        [TestMethod]
        public void Banco_DeveValidarNivelAcademico()
        {
            Assert.ThrowsException<SqlException>(() => Executar(
                "UPDATE dbo.Estudante SET NivelAcademico = N'Nível inexistente' WHERE EstudanteId = 7;"));
        }

        [TestMethod]
        public void Banco_DeveGarantirLoginUnico()
        {
            Assert.ThrowsException<SqlException>(() => Executar(@"
INSERT dbo.Usuario (Login, NomeCompleto, Email, SenhaHash, PerfilId, Ativo)
VALUES (N'admin', N'Outro Admin', N'outro.admin@gpfa.local', N'120000.x.y', 1, 1);"));
        }

        [TestMethod]
        public void Banco_NaoDevePermitirDoisDestaquesNaMesmaPosicao()
        {
            Assert.ThrowsException<SqlException>(() => Executar(@"
INSERT dbo.DestaquePublicacao (PesquisadorId, PublicacaoId, Ordem)
VALUES (1, 7, 1);"));
        }

        private static void Executar(string sql)
        {
            using (var conexao = TestDatabase.Abrir())
            using (var comando = new SqlCommand(sql, conexao))
            {
                comando.ExecuteNonQuery();
            }
        }
    }
}
