using System.Data.SqlClient;
using GPFA.Models;

namespace GPFA.Data
{
    internal static class Mappers
    {
        public static ProjetoResumo ProjetoResumo(SqlDataReader r)
        {
            return new ProjetoResumo
            {
                ProjetoId = DbValue.Int(r, "ProjetoId"),
                Titulo = DbValue.String(r, "Titulo"),
                Sigla = DbValue.String(r, "Sigla"),
                Resumo = DbValue.String(r, "Resumo"),
                Descricao = DbValue.String(r, "Descricao"),
                LinhaPesquisa = DbValue.String(r, "LinhaPesquisa"),
                Situacao = DbValue.String(r, "Situacao"),
                DataInicio = DbValue.Date(r, "DataInicio"),
                DataFimPrevista = DbValue.DateNull(r, "DataFimPrevista"),
                DataFim = DbValue.DateNull(r, "DataFim"),
                CoordenadorId = DbValue.Int(r, "CoordenadorId"),
                CoordenadorNome = DbValue.String(r, "CoordenadorNome"),
                CoordenadorFotoUrl = DbValue.String(r, "CoordenadorFotoUrl"),
                ImagemUrl = DbValue.String(r, "ImagemUrl"),
                Destaque = DbValue.Bool(r, "Destaque"),
                QtdPesquisadores = DbValue.Int(r, "QtdPesquisadores"),
                QtdEstudantes = DbValue.Int(r, "QtdEstudantes"),
                QtdPublicacoes = DbValue.Int(r, "QtdPublicacoes"),
                TotalCaptado = DbValue.Decimal(r, "TotalCaptado"),
                CriadoEm = DbValue.Date(r, "CriadoEm"),
                AtualizadoEm = DbValue.Date(r, "AtualizadoEm")
            };
        }

        public static PesquisadorResumo PesquisadorResumo(SqlDataReader r)
        {
            return new PesquisadorResumo
            {
                PesquisadorId = DbValue.Int(r, "PesquisadorId"),
                NomeCompleto = DbValue.String(r, "NomeCompleto"),
                Email = DbValue.String(r, "Email"),
                FotoUrl = DbValue.String(r, "FotoUrl"),
                FormacaoAcademica = DbValue.String(r, "FormacaoAcademica"),
                AreaAtuacao = DbValue.String(r, "AreaAtuacao"),
                MiniBiografia = DbValue.String(r, "MiniBiografia"),
                GoogleScholarUrl = DbValue.String(r, "GoogleScholarUrl"),
                OrcidUrl = DbValue.String(r, "OrcidUrl"),
                LattesUrl = DbValue.String(r, "LattesUrl"),
                Ativo = DbValue.Bool(r, "Ativo"),
                Titulacao = DbValue.String(r, "Titulacao"),
                Instituicao = DbValue.String(r, "Instituicao"),
                Departamento = DbValue.String(r, "Departamento"),
                LinhaPesquisa = DbValue.String(r, "LinhaPesquisa"),
                QtdProjetosCoordenados = DbValue.Int(r, "QtdProjetosCoordenados"),
                QtdProjetos = DbValue.Int(r, "QtdProjetos"),
                QtdOrientandosAtuais = DbValue.Int(r, "QtdOrientandosAtuais"),
                QtdOrientandosPassados = DbValue.Int(r, "QtdOrientandosPassados"),
                QtdPublicacoes = DbValue.Int(r, "QtdPublicacoes"),
                TotalCaptado = DbValue.Decimal(r, "TotalCaptado")
            };
        }

        public static EstudanteResumo EstudanteResumo(SqlDataReader r)
        {
            return new EstudanteResumo
            {
                EstudanteId = DbValue.Int(r, "EstudanteId"),
                NomeCompleto = DbValue.String(r, "NomeCompleto"),
                Email = DbValue.String(r, "Email"),
                FotoUrl = DbValue.String(r, "FotoUrl"),
                FormacaoAcademica = DbValue.String(r, "FormacaoAcademica"),
                AreaAtuacao = DbValue.String(r, "AreaAtuacao"),
                NivelAcademico = DbValue.String(r, "NivelAcademico"),
                Curso = DbValue.String(r, "Curso"),
                Situacao = DbValue.String(r, "Situacao"),
                DataIngresso = DbValue.Date(r, "DataIngresso"),
                DataConclusao = DbValue.DateNull(r, "DataConclusao"),
                CoorientadoresExternos = DbValue.String(r, "CoorientadoresExternos"),
                OrientadorId = DbValue.IntNull(r, "OrientadorId"),
                OrientadorNome = DbValue.String(r, "OrientadorNome"),
                QtdProjetosAtivos = DbValue.Int(r, "QtdProjetosAtivos"),
                ProjetosAtivos = DbValue.String(r, "ProjetosAtivos")
            };
        }

        public static OrientandoResumo Orientando(SqlDataReader r)
        {
            return new OrientandoResumo
            {
                OrientacaoId = DbValue.Int(r, "OrientacaoId"),
                EstudanteId = DbValue.Int(r, "EstudanteId"),
                EstudanteNome = DbValue.String(r, "EstudanteNome"),
                EstudanteEmail = DbValue.String(r, "EstudanteEmail"),
                EstudanteFotoUrl = DbValue.String(r, "EstudanteFotoUrl"),
                NivelAcademico = DbValue.String(r, "NivelAcademico"),
                Curso = DbValue.String(r, "Curso"),
                Situacao = DbValue.String(r, "Situacao"),
                CoorientadoresExternos = DbValue.String(r, "CoorientadoresExternos"),
                DataInicio = DbValue.Date(r, "DataInicio"),
                DataFim = DbValue.DateNull(r, "DataFim"),
                Vinculo = DbValue.String(r, "Vinculo")
            };
        }

        public static Orientacao Orientacao(SqlDataReader r)
        {
            return new Orientacao
            {
                OrientacaoId = DbValue.Int(r, "OrientacaoId"),
                EstudanteId = DbValue.Int(r, "EstudanteId"),
                EstudanteNome = DbValue.String(r, "EstudanteNome"),
                OrientadorId = DbValue.Int(r, "OrientadorId"),
                OrientadorNome = DbValue.String(r, "OrientadorNome"),
                Principal = DbValue.Bool(r, "Principal"),
                DataInicio = DbValue.Date(r, "DataInicio"),
                DataFim = DbValue.DateNull(r, "DataFim"),
                Observacao = DbValue.String(r, "Observacao"),
                EstudanteNivel = DbValue.String(r, "EstudanteNivel"),
                EstudanteSituacao = DbValue.String(r, "EstudanteSituacao")
            };
        }

        public static PublicacaoResumo PublicacaoResumo(SqlDataReader r)
        {
            return new PublicacaoResumo
            {
                PublicacaoId = DbValue.Int(r, "PublicacaoId"),
                Titulo = DbValue.String(r, "Titulo"),
                Veiculo = DbValue.String(r, "Veiculo"),
                Ano = DbValue.Int(r, "Ano"),
                Tipo = DbValue.String(r, "Tipo"),
                Doi = DbValue.String(r, "Doi"),
                LinkAcesso = DbValue.String(r, "LinkAcesso"),
                ProjetoId = DbValue.IntNull(r, "ProjetoId"),
                ProjetoTitulo = DbValue.String(r, "ProjetoTitulo"),
                ProjetoSigla = DbValue.String(r, "ProjetoSigla"),
                Autores = DbValue.String(r, "Autores"),
                CriadoEm = DbValue.Date(r, "CriadoEm")
            };
        }

        public static Publicacao PublicacaoEntity(SqlDataReader r)
        {
            return new Publicacao
            {
                PublicacaoId = DbValue.Int(r, "PublicacaoId"),
                Titulo = DbValue.String(r, "Titulo"),
                Veiculo = DbValue.String(r, "Veiculo"),
                Ano = DbValue.Int(r, "Ano"),
                Tipo = DbValue.String(r, "Tipo"),
                Doi = DbValue.String(r, "Doi"),
                LinkAcesso = DbValue.String(r, "LinkAcesso"),
                ProjetoId = DbValue.IntNull(r, "ProjetoId"),
                ProjetoTitulo = DbValue.String(r, "ProjetoTitulo"),
                ProjetoSigla = DbValue.String(r, "ProjetoSigla"),
                CriadoEm = DbValue.Date(r, "CriadoEm")
            };
        }

        public static FinanciamentoResumo FinanciamentoResumo(SqlDataReader r)
        {
            return new FinanciamentoResumo
            {
                FinanciamentoId = DbValue.Int(r, "FinanciamentoId"),
                ProjetoId = DbValue.Int(r, "ProjetoId"),
                ProjetoTitulo = DbValue.String(r, "ProjetoTitulo"),
                FonteId = DbValue.Int(r, "FonteId"),
                FonteNome = DbValue.String(r, "FonteNome"),
                FonteSigla = DbValue.String(r, "FonteSigla"),
                Descricao = DbValue.String(r, "Descricao"),
                NumeroProcesso = DbValue.String(r, "NumeroProcesso"),
                Valor = DbValue.Decimal(r, "Valor"),
                DataInicio = DbValue.Date(r, "DataInicio"),
                DataFim = DbValue.DateNull(r, "DataFim")
            };
        }

        public static Financiamento FinanciamentoEntity(SqlDataReader r)
        {
            return new Financiamento
            {
                FinanciamentoId = DbValue.Int(r, "FinanciamentoId"),
                ProjetoId = DbValue.Int(r, "ProjetoId"),
                ProjetoTitulo = DbValue.String(r, "ProjetoTitulo"),
                ProjetoSigla = DbValue.String(r, "ProjetoSigla"),
                FonteId = DbValue.Int(r, "FonteId"),
                FonteNome = DbValue.String(r, "FonteNome"),
                FonteSigla = DbValue.String(r, "FonteSigla"),
                Descricao = DbValue.String(r, "Descricao"),
                NumeroProcesso = DbValue.String(r, "NumeroProcesso"),
                Valor = DbValue.Decimal(r, "Valor"),
                DataInicio = DbValue.Date(r, "DataInicio"),
                DataFim = DbValue.DateNull(r, "DataFim"),
                Moeda = DbValue.String(r, "Moeda"),
                CriadoEm = DbValue.Date(r, "CriadoEm")
            };
        }

        public static FonteFinanciadora Fonte(SqlDataReader r)
        {
            return new FonteFinanciadora
            {
                FonteId = DbValue.Int(r, "FonteId"),
                Nome = DbValue.String(r, "Nome"),
                Sigla = DbValue.String(r, "Sigla"),
                Tipo = DbValue.String(r, "Tipo"),
                SiteUrl = DbValue.String(r, "SiteUrl"),
                QtdFinanciamentos = DbValue.Int(r, "QtdFinanciamentos"),
                QtdProjetos = DbValue.Int(r, "QtdProjetos"),
                TotalCaptado = DbValue.Decimal(r, "TotalCaptado")
            };
        }

        public static Pesquisador PesquisadorEntity(SqlDataReader r)
        {
            return new Pesquisador
            {
                PessoaId = DbValue.Int(r, "PessoaId"),
                NomeCompleto = DbValue.String(r, "NomeCompleto"),
                Email = DbValue.String(r, "Email"),
                TipoPessoa = 'P',
                FotoUrl = DbValue.String(r, "FotoUrl"),
                FormacaoAcademica = DbValue.String(r, "FormacaoAcademica"),
                AreaAtuacao = DbValue.String(r, "AreaAtuacao"),
                MiniBiografia = DbValue.String(r, "MiniBiografia"),
                GoogleScholarUrl = DbValue.String(r, "GoogleScholarUrl"),
                OrcidUrl = DbValue.String(r, "OrcidUrl"),
                LattesUrl = DbValue.String(r, "LattesUrl"),
                Ativo = DbValue.Bool(r, "Ativo"),
                Titulacao = DbValue.String(r, "Titulacao"),
                Instituicao = DbValue.String(r, "Instituicao"),
                Departamento = DbValue.String(r, "Departamento"),
                LinhaPesquisa = DbValue.String(r, "LinhaPesquisa"),
                DataCadastro = DbValue.Date(r, "DataCadastro")
            };
        }

        public static Estudante EstudanteEntity(SqlDataReader r)
        {
            return new Estudante
            {
                PessoaId = DbValue.Int(r, "PessoaId"),
                NomeCompleto = DbValue.String(r, "NomeCompleto"),
                Email = DbValue.String(r, "Email"),
                TipoPessoa = 'E',
                FotoUrl = DbValue.String(r, "FotoUrl"),
                FormacaoAcademica = DbValue.String(r, "FormacaoAcademica"),
                AreaAtuacao = DbValue.String(r, "AreaAtuacao"),
                MiniBiografia = DbValue.String(r, "MiniBiografia"),
                GoogleScholarUrl = DbValue.String(r, "GoogleScholarUrl"),
                OrcidUrl = DbValue.String(r, "OrcidUrl"),
                LattesUrl = DbValue.String(r, "LattesUrl"),
                Ativo = DbValue.Bool(r, "Ativo"),
                NivelAcademico = DbValue.String(r, "NivelAcademico"),
                Curso = DbValue.String(r, "Curso"),
                Situacao = DbValue.String(r, "Situacao"),
                DataIngresso = DbValue.Date(r, "DataIngresso"),
                DataConclusao = DbValue.DateNull(r, "DataConclusao"),
                CoorientadoresExternos = DbValue.String(r, "CoorientadoresExternos")
            };
        }

        public static Usuario Usuario(SqlDataReader r)
        {
            return new Usuario
            {
                UsuarioId = DbValue.Int(r, "UsuarioId"),
                Login = DbValue.String(r, "Login"),
                NomeCompleto = DbValue.String(r, "NomeCompleto"),
                Email = DbValue.String(r, "Email"),
                SenhaHash = DbValue.String(r, "SenhaHash"),
                PerfilId = DbValue.Int(r, "PerfilId"),
                PerfilNome = DbValue.String(r, "PerfilNome"),
                PesquisadorId = DbValue.IntNull(r, "PesquisadorId"),
                PesquisadorNome = DbValue.String(r, "PesquisadorNome"),
                Ativo = DbValue.Bool(r, "Ativo"),
                UltimoAcesso = DbValue.DateNull(r, "UltimoAcesso"),
                TentativasFalhas = DbValue.Int(r, "TentativasFalhas"),
                BloqueadoAte = DbValue.DateNull(r, "BloqueadoAte"),
                CriadoEm = DbValue.Date(r, "CriadoEm")
            };
        }

        public static ParticipacaoProjeto Participacao(SqlDataReader r)
        {
            return new ParticipacaoProjeto
            {
                ParticipacaoId = DbValue.Int(r, "ParticipacaoId"),
                ProjetoId = DbValue.Int(r, "ProjetoId"),
                PesquisadorId = DbValue.Int(r, "PesquisadorId"),
                PesquisadorNome = DbValue.String(r, "PesquisadorNome"),
                PesquisadorFotoUrl = DbValue.String(r, "PesquisadorFotoUrl"),
                Papel = DbValue.String(r, "Papel"),
                DataEntrada = DbValue.Date(r, "DataEntrada"),
                DataSaida = DbValue.DateNull(r, "DataSaida")
            };
        }

        public static DestaqueAdminResumo DestaqueAdmin(SqlDataReader r)
        {
            return new DestaqueAdminResumo
            {
                DestaqueId = DbValue.Int(r, "DestaqueId"),
                PesquisadorId = DbValue.Int(r, "PesquisadorId"),
                PesquisadorNome = DbValue.String(r, "PesquisadorNome"),
                PublicacaoId = DbValue.Int(r, "PublicacaoId"),
                Titulo = DbValue.String(r, "Titulo"),
                Veiculo = DbValue.String(r, "Veiculo"),
                Ano = DbValue.Int(r, "Ano"),
                Ordem = DbValue.Int(r, "Ordem")
            };
        }

        public static LogAuditoria Log(SqlDataReader r)
        {
            return new LogAuditoria
            {
                LogId = r.GetInt64(r.GetOrdinal("LogId")),
                UsuarioId = DbValue.IntNull(r, "UsuarioId"),
                UsuarioNome = DbValue.String(r, "UsuarioNome"),
                Acao = DbValue.String(r, "Acao"),
                Entidade = DbValue.String(r, "Entidade"),
                EntidadeId = DbValue.IntNull(r, "EntidadeId"),
                Detalhe = DbValue.String(r, "Detalhe"),
                DataHora = DbValue.Date(r, "DataHora")
            };
        }
    }
}
