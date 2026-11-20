using System;
using System.Collections.Generic;
using GPFA.Business;
using GPFA.Models;

namespace GPFA.Tests
{
    internal static class TestData
    {
        public static string Sufixo()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public static UsuarioSessao Admin()
        {
            return new UsuarioSessao
            {
                UsuarioId = 1,
                Login = "admin",
                NomeCompleto = "Administração GPFA",
                PerfilId = Dominio.Perfil.Administrador,
                PerfilNome = Dominio.Perfil.AdministradorNome
            };
        }

        public static UsuarioSessao CoordenadorHelena()
        {
            return new UsuarioSessao
            {
                UsuarioId = 2,
                Login = "coordenador",
                NomeCompleto = "Helena Marques Ferreira",
                PerfilId = Dominio.Perfil.Coordenador,
                PerfilNome = Dominio.Perfil.CoordenadorNome,
                PesquisadorId = 1
            };
        }

        public static Pesquisador NovoPesquisador(string nome = null, bool ativo = true)
        {
            var sufixo = Sufixo();
            return new Pesquisador
            {
                NomeCompleto = nome ?? ("Pesquisador Teste " + sufixo),
                Email = "pesq." + sufixo + "@teste.gpfa.local",
                FormacaoAcademica = "Doutorado em Ciências (teste)",
                AreaAtuacao = "Área de Teste",
                Titulacao = "Doutor(a)",
                Instituicao = "UFMG",
                Departamento = "Departamento de Teste",
                LinhaPesquisa = "Linha de Teste",
                MiniBiografia = "Pesquisador criado por teste automatizado.",
                Ativo = ativo
            };
        }

        public static int CriarPesquisador(string nome = null, bool ativo = true)
        {
            return AppServices.Pesquisadores.Salvar(NovoPesquisador(nome, ativo));
        }

        public static Projeto NovoProjeto(int coordenadorId, string situacao = null)
        {
            var sufixo = Sufixo();
            return new Projeto
            {
                Titulo = "Projeto de Teste " + sufixo,
                Sigla = "TST-" + sufixo.Substring(0, 4),
                Resumo = "Projeto criado automaticamente pela suíte de testes de integração.",
                Descricao = "Descrição do projeto de teste.",
                LinhaPesquisa = "Linha de Teste",
                Situacao = situacao ?? Dominio.SituacaoProjeto.EmAndamento,
                DataInicio = DateTime.Today.AddMonths(-6),
                DataFimPrevista = DateTime.Today.AddMonths(18),
                CoordenadorId = coordenadorId,
                Destaque = false
            };
        }

        public static int CriarProjeto(int coordenadorId, UsuarioSessao usuario = null,
            string situacao = null)
        {
            return AppServices.Projetos.Salvar(NovoProjeto(coordenadorId, situacao), usuario ?? Admin());
        }

        public static Estudante NovoEstudante()
        {
            var sufixo = Sufixo();
            return new Estudante
            {
                NomeCompleto = "Estudante Teste " + sufixo,
                Email = "est." + sufixo + "@teste.gpfa.local",
                NivelAcademico = Dominio.NivelAcademico.Mestrado,
                Curso = "Física",
                DataIngresso = DateTime.Today.AddMonths(-8),
                CoorientadoresExternos = "Prof. Externo de Teste",
                FormacaoAcademica = "Graduação em Física (teste)",
                AreaAtuacao = "Física",
                Ativo = true
            };
        }

        public static int CriarEstudante(int orientadorId, int projetoId, string tipoVinculo = "Bolsa de teste")
        {
            return AppServices.Estudantes.Criar(NovoEstudante(), orientadorId, new List<int> { projetoId }, tipoVinculo);
        }

        public static Publicacao NovaPublicacao(int autorId, int? projetoId = null, string titulo = null)
        {
            var sufixo = Sufixo();
            var publicacao = new Publicacao
            {
                Titulo = titulo ?? ("Publicação de Teste " + sufixo),
                Veiculo = "Revista de Testes",
                Ano = DateTime.Today.Year,
                Tipo = Dominio.TipoPublicacao.Artigo,
                Doi = "10.9999/teste." + sufixo,
                LinkAcesso = "https://example.org/teste/" + sufixo,
                ProjetoId = projetoId
            };
            publicacao.Autores.Add(new PublicacaoAutor { PesquisadorId = autorId });
            return publicacao;
        }

        public static int CriarPublicacao(int autorId, int? projetoId = null, string titulo = null)
        {
            return AppServices.Publicacoes.Salvar(NovaPublicacao(autorId, projetoId, titulo), Admin());
        }

        public static int CriarFonte()
        {
            return AppServices.Financiamentos.SalvarFonte(new FonteFinanciadora
            {
                Nome = "Fonte de Teste " + Sufixo(),
                Sigla = "FT" + Sufixo().Substring(0, 3).ToUpperInvariant(),
                Tipo = Dominio.TipoFonte.Outra,
                SiteUrl = "https://example.org/fonte"
            }, Admin());
        }

        public static int CriarFinanciamento(int projetoId, int fonteId, decimal valor,
            string processo = null, UsuarioSessao usuario = null)
        {
            return AppServices.Financiamentos.Salvar(new Financiamento
            {
                ProjetoId = projetoId,
                FonteId = fonteId,
                Descricao = "Financiamento de teste",
                NumeroProcesso = processo,
                Valor = valor,
                DataInicio = DateTime.Today.AddMonths(-3),
                DataFim = DateTime.Today.AddMonths(9),
                Moeda = "BRL"
            }, usuario ?? Admin());
        }

        public static string CriarUsuario(int perfilId, int? pesquisadorId, string senha = "Teste@123")
        {
            var sufixo = Sufixo();
            var login = "usuario." + sufixo;
            AppServices.Usuarios.Salvar(new Usuario
            {
                Login = login,
                NomeCompleto = "Usuário Teste " + sufixo,
                Email = login + "@teste.gpfa.local",
                PerfilId = perfilId,
                PesquisadorId = pesquisadorId,
                Ativo = true
            }, senha, Admin());
            return login;
        }
    }
}
